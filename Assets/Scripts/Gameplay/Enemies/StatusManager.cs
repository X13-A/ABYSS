using System;
using System.Collections.Generic;
using System.Text;

public class StatusManager
{
    private HashSet<EnemyStatus> status;

    public StatusManager()
    {
        status = new HashSet<EnemyStatus>();
    }

    public void SetStatus(EnemyStatus s)
    {
        status.Add(s);
    }

    public void UnsetStatus(EnemyStatus s)
    {
        status.Remove(s);
    }

    public bool HasStatus(EnemyStatus s)
    {
        return status.Contains(s);
    }

    public void ClearStatus()
    {
        status.Clear();
    }

    public List<EnemyStatus> GetAllStatus()
    {
        return new List<EnemyStatus>(status);
    }

    override
    public string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (EnemyStatus status in status)
        {
            stringBuilder.Append(status.ToString());
            stringBuilder.Append("\n");
        }
        return stringBuilder.ToString();
    }

}

