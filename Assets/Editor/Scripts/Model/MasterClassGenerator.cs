using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;

/// <summary>
/// テーブル定義書から自動でModelクラスを作成する
///
/// 事前準備
/// ・t4コマンドのパスを通しておく（~\dotnet\toolsにインストールされる）
/// 　インストール:dotnet tool install dotnet-t4 -g --version 2.2.1
/// ・MasterTable.ttのassemblyで絶対パスを設定する
/// </summary>
public class MasterClassGenerator : EditorWindow {

    static readonly string DefaultExcelFilePath = "./Document/MasterDefinition.xlsx";
    static readonly string TemplateFolder = $"{Application.dataPath}/Editor/Template";
    static readonly string MasterTemplatePath = $"{TemplateFolder}/MasterClass.tt";
    static readonly string MasterHelperClassName = "MasterHelper";
    static readonly string MasterHelperTemplatePath = $"{TemplateFolder}/{MasterHelperClassName}.tt";
    static readonly string OutputFolder = $"{Application.dataPath}/Scripts/Model";
    static readonly string MasterPrefix = "Mst";


    [MenuItem("Tools/MasterClassGenerator")]
    public static void ShowWindow() {
        GetWindow<MasterClassGenerator>("モデルクラス作成");
    }

    void OnGUI() {
        GUILayout.Label("テーブル定義からモデルクラス作成", EditorStyles.boldLabel);

        GUILayout.Label("Excel File Path:");
        string filePath = EditorGUILayout.TextField(DefaultExcelFilePath);

        if (GUILayout.Button("Select Excel File")) {
            filePath = EditorUtility.OpenFilePanel("Select Excel File", "", "xlsx");
        }

        if (GUILayout.Button("Generate")) {
            if (string.IsNullOrEmpty(filePath)) {
                EditorUtility.DisplayDialog("Error", "Please select an Excel file.", "OK");
                return;
            }

            var tableDefinitions = GenerateModels(filePath, MasterPrefix);

            // マスタクラス作成
            {
                foreach (var tableDefinition in tableDefinitions) {
                    string outputPath =
                        $"{OutputFolder}/{MasterPrefix}/{tableDefinition.Prefix}{tableDefinition.TableName.ToUpperFirstLetter()}.cs";
                    string param = JsonConvert.SerializeObject(tableDefinition).Replace("\"", "\\\"");
                    if (!Util.ExecuteCommand(
                            "t4",
                            $"-o=\"{outputPath}\" -p=jsonStr=\"{param}\" \"{MasterTemplatePath}\"")) {
                        return;
                    }
                }
            }

            // ヘルパークラス作成
            {
                string outputPath =
                    $"{OutputFolder}/{MasterPrefix}/{MasterHelperClassName}.cs";
                string param = JsonConvert.SerializeObject(tableDefinitions).Replace("\"", "\\\"");
                if (!Util.ExecuteCommand(
                        "t4",
                        $"-o=\"{outputPath}\" -p=jsonStr=\"{param}\" \"{MasterHelperTemplatePath}\"")) {
                    return;
                }
            }

            // master memoryのコマンド実行
            {
                if (!Util.ExecuteCommand(
                        "dotnet-mmgen",
                        $"-inputDirectory {OutputFolder}/{MasterPrefix} " +
                        $"-outputDirectory {OutputFolder}/{MasterPrefix}/Generated -c -usingNamespace Model")) {
                    return;
                }
                if (!Util.ExecuteCommand(
                        "mpc",
                        $"-input {OutputFolder}/{MasterPrefix} -output {OutputFolder}/{MasterPrefix}/Generated")) {
                    return;
                }
            }
        }
    }

    List<TableDefinition> GenerateModels(string filePath, String prefix) {
        var result = new List<TableDefinition>();
        using FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        IWorkbook workbook = new XSSFWorkbook(file);
        for (int sheetIdx = 0; sheetIdx < workbook.NumberOfSheets; sheetIdx++) {
            ISheet sheet = workbook.GetSheetAt(sheetIdx);
            result.Add(GenerateModel(sheet, prefix));
        }
        return result;
    }

    TableDefinition GenerateModel(ISheet sheet, String prefix) {
        // Index
        Dictionary<string, List<IndexDefinition>> indexDict = new();
        int startColumn;
        for (int row = 1; row <= sheet.LastRowNum; row++) // 1行目はヘッダーのためスキップ
        {
            startColumn = 5; // 列G
            IRow currentRow = sheet.GetRow(row);
            if (currentRow?.GetCell(startColumn) == null)
                break;
            bool isUnique = currentRow.GetCell(startColumn++).BooleanCellValue;
            // 複合キーは5つまで
            for (int i = 0; i < 5; i++) {
                string key = currentRow.GetCell(startColumn++)?.StringCellValue;
                if (key == null) {
                    break;
                }
                indexDict.GetValueOrDefault(key)
                    .Add(new IndexDefinition(row - 1, i, isUnique));
            }
        }

        // カラム定義
        List<ColumnDefinition> columns = new();
        for (int row = 1; row <= sheet.LastRowNum; row++) // 1行目はヘッダーのためスキップ
        {
            startColumn = 0; // 列A
            IRow currentRow = sheet.GetRow(row);
            if (currentRow?.GetCell(startColumn) == null)
                break;
            string columnName = currentRow.GetCell(startColumn++).StringCellValue;
            string dataType = currentRow.GetCell(startColumn++).StringCellValue;
            columns.Add(new ColumnDefinition(columnName, dataType,
                indexDict.GetValueOrDefault(columnName)));
            indexDict.Remove(columnName);
        }

        if (indexDict.Count > 0) {
            foreach (string column in indexDict.Keys) {
                Debug.LogError($"indexに不正な値。sheetName={sheet.SheetName}, columnName={column}");
            }
        }

        TableDefinition table = new TableDefinition(prefix, sheet.SheetName, columns);
        return table;
    }
}

public record TableDefinition(string Prefix, string TableName, List<ColumnDefinition> Columns) {
    public string TableName { get; } = TableName;
    public string Prefix { get; } = Prefix;
    public List<ColumnDefinition> Columns { get; } = Columns;
}

public record ColumnDefinition(
    string Name,
    string DataTypeDb,
    List<IndexDefinition> Indexes
) {
    public string Name { get; } = Name;
    public string DataTypeCs { get; } = ConvertDataTypeToCsharp(DataTypeDb);
    public string DataTypeDb { get; } = DataTypeDb;
    public List<IndexDefinition> Indexes { get; } = Indexes;

    static string ConvertDataTypeToCsharp(string dataTypeDb) {
        if (dataTypeDb.StartsWith("varchar")) {
            return "string";
        }
        return dataTypeDb;
    }
}

public record IndexDefinition(int Id, int KeyOrder, bool IsUnique) {
    public int Id { get; } = Id;
    public bool IsPk { get; } = Id == 0;
    public bool IsUnique { get; } = IsUnique;
    public int KeyOrder { get; } = KeyOrder;
}