using System;
using UnityEngine;

public class AIGrid<TAIGridObject>
{
    public event EventHandler<OnAIGridObjectChangedEventArgs> OnAIGridObjectChanged;
    public class OnAIGridObjectChangedEventArgs : EventArgs { public int x, z; }

    private int width, height;
    private float cellSize;
    private Vector3 originPosition;
    private TAIGridObject[,] gridArray;
    private TextMesh[,] debugArray;

    public AIGrid(int width, int height, float cellSize, Vector3 originPosition, Func<AIGrid<TAIGridObject>, int, int, Vector3, TAIGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TAIGridObject[width, height];
        debugArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this, x, z, GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * 0.5f);
            }
        }

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                debugArray[x, z] = AIUtils.CreateWorldText(gridArray[x, z]?.ToString(), null, 
                    GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * 0.5f, 10, Color.white, TextAnchor.MiddleCenter);

                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    public int GetWidth() => width;
    public int GetHeight() => height;

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetAIGridObject(int x, int z, TAIGridObject value)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x, z] = value;
            debugArray[x, z].text = gridArray[x, z]?.ToString();
            OnAIGridObjectChanged?.Invoke(this, new OnAIGridObjectChangedEventArgs { x = x, z = z });
        }
    }

    public void TriggerAIGridObjectChanged(int x, int z)
    {
        OnAIGridObjectChanged?.Invoke(this, new OnAIGridObjectChangedEventArgs { x = x, z = z });
    }

    public void SetAIGridObject(Vector3 worldPosition, TAIGridObject value)
    {
        GetXZ(worldPosition, out int x, out int z);
        SetAIGridObject(x, z, value);
    }

    public TAIGridObject GetAIGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];
        }
        return default; // Same as: return default(TAIGridObject);
    }

    public TAIGridObject GetAIGridObject(Vector3 worldPosition)
    {
        GetXZ(worldPosition, out int x, out int z);
        return GetAIGridObject(x, z);
    }

    private Vector3 GetWorldPosition(int x, int z) => new Vector3(x, 0, z) * cellSize + originPosition;

}
