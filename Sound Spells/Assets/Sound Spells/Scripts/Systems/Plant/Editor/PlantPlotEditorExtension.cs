using Sound_Spells.Models.Plant;
using UnityEditor;
using UnityEngine;

namespace Sound_Spells.Systems.Plant.Editor
{
    [CustomEditor(typeof(PlantPlot))]
    public class PlantPlotEditorExtension : UnityEditor.Editor
    {
        private PlantData _selectedPlantData;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var plantPlot = (PlantPlot)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Plant Management", EditorStyles.boldLabel);

            _selectedPlantData = (PlantData)EditorGUILayout.ObjectField("Plant Data", _selectedPlantData, typeof(PlantData), false);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Sow"))
            {
                if (_selectedPlantData)
                {
                    plantPlot.SowPlant(_selectedPlantData);
                }
                else
                {
                    Debug.LogWarning("Please select a PlantData asset before sowing.");
                }
            }

            if (GUILayout.Button("Remove"))
            {
                plantPlot.RemovePlant();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
