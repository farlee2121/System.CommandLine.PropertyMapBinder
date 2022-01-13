using KellermanSoftware.CompareNetObjects;
using Xunit.Sdk;

namespace NameConventionBinder.Tests;

public class DeepAssert
{
    public static void Equal<T>(T expected, T actual)
    {
        CompareLogic compareLogic = new CompareLogic();

        var comparisonResult = compareLogic.Compare(expected, actual);

        if(comparisonResult.AreEqual == false)
        {
            throw new ObjectEqualException(expected, actual, comparisonResult.DifferencesString);
        }
    }

    public class ObjectEqualException : AssertActualExpectedException
    {
        private readonly string message;

        public ObjectEqualException(object expected, object actual, string message)
            : base(expected, actual, "Assert.Equal() Failure")
        {
            this.message = message;
        }

        public override string Message => message;
    }
}