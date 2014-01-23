namespace Regalo.ObjectCompare
{
    public interface IObjectComparer
    {
        ObjectComparisonResult AreEqual(object object1, object object2);
    }
}