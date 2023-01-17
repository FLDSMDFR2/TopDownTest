[System.Serializable]
public class BaseSaveableObj
{
    protected string path;

    public virtual string Path { get { return path; } }

    public virtual void Save()
    {

    }

    public virtual T Load<T>()
    {
        return default(T);
    }
}