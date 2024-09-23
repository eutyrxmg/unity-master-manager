using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MasterMemory;
using MessagePack;
using MessagePack.Resolvers;
using Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Excelからマスタデータを読み込み、Master Memoryのバイナリファイルとして保存する
/// </summary>
public class MasterDataGenerator : EditorWindow {

    static readonly string DefaultExcelFilePath = "./Document/MasterData.xlsx";

    [MenuItem("Tools/MasterDataGenerator")]
    public static void ShowWindow() {
        GetWindow<MasterDataGenerator>("マスタデータ作成");
    }

    void OnGUI() {
        GUILayout.Label("Excelからマスタデータ作成", EditorStyles.boldLabel);

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
            
            var tableData = LoadMasterTables(filePath);
            
            SetupMessagePackResolver();
            var databaseBuilder = new DatabaseBuilder();
            var metaDatabase = MemoryDatabase.GetMetaDatabase();
            foreach (var metaTable in metaDatabase.GetTableInfos()) {
                databaseBuilder.AppendDynamic(metaTable.DataType, tableData[metaTable.TableName]);
            }
            Util.SaveResource(Const.MasterDataPath, databaseBuilder.Build());

            Debug.Log("MasterDataGenerator completed.");
        }
    }

    Dictionary<string, List<object>> LoadMasterTables(string filePath) {
        Dictionary<string, List<object>> result = new();
        using FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        IWorkbook workbook = new XSSFWorkbook(file);
        for (int sheetIdx = 0; sheetIdx < workbook.NumberOfSheets; sheetIdx++) {
            ISheet sheet = workbook.GetSheetAt(sheetIdx);
            result.Add(sheet.SheetName, LoadMasterTable(sheet));
        }
        return result;
    }

    List<object> LoadMasterTable(ISheet sheet) {
        string tableName = sheet.SheetName;
        var constructor = MasterHelper.GetConstructor(tableName);
        IRow headerRow = sheet.GetRow(0);
        List<string> columnNames =
            headerRow.Cells.Select(cell => cell.StringCellValue).Where(str => str.Length > 0).ToList();
        List<object> result = new();
        for (int row = 1; row <= sheet.LastRowNum; row++) {
            IRow currentRow = sheet.GetRow(row);
            if (currentRow?.GetCell(0) == null) {
                break;
            }
            
            var instance = constructor();
            Type type = instance.GetType();
            // カラム名から対象のフィールドを探して、reflectionを使用して値を保存
            for (int i = 0; i < columnNames.Count; i++) {
                string columnName = columnNames[i];
                ICell cell = currentRow.GetCell(i);
                PropertyInfo property = type.GetProperty(columnName.ToUpperFirstLetter(),
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (property == null) {
                    throw new Exception($"存在しないフィールド class={type.Name}, field={columnName}");
                }
                try {
                    property.SetValue(instance, GetCellValue(cell, property.PropertyType));
                } catch (Exception e) {
                    Debug.LogError($"tableName={tableName}, columnName={columnName}, row={row} | ${e}");
                }
            }
            result.Add(instance);
        }
        return result;
    }

    object GetCellValue(ICell cell, Type type) {
        return Type.GetTypeCode(type) switch {
            TypeCode.String => cell.StringCellValue,
            TypeCode.Boolean => cell.BooleanCellValue,
            TypeCode.DateTime => cell.DateCellValue,
            TypeCode.Double => cell.NumericCellValue,
            _ => (int)cell.NumericCellValue
        };
    }
    
    static void SetupMessagePackResolver() {
        StaticCompositeResolver.Instance.Register(new IFormatterResolver[] {
            MasterMemoryResolver.Instance, // set MasterMemory generated resolver
            GeneratedResolver.Instance, // set MessagePack generated resolver
            StandardResolver.Instance // set default MessagePack resolver
        });

        var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
        MessagePackSerializer.DefaultOptions = options;
    }
}