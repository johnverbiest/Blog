using BlogExamples.Dependencies;

namespace BlogExamples
{
    public class AwesomeClass : IAwesomeClass
    {
        private readonly IDependency1 _dep1;
        private readonly IDependency2 _dep2;
        private readonly IDependency3 _dep3;
        private readonly IDependency4 _dep4;

        public AwesomeClass(IDependency1 dep1, IDependency2 dep2 ,IDependency3 dep3, IDependency4 dep4)
        {
            _dep1 = dep1;
            _dep2 = dep2;
            _dep3 = dep3;
            _dep4 = dep4;
        }

        public string DoAThing(string withThis, int withThat)
        {
            var someResult = _dep1.Result(withThis);
            someResult += withThat;
            return someResult.ToString();
        }

        public string DoAnotherThing(string withThis, int withThat)
        {
            var someResult = _dep2.Result(withThis);
            someResult += withThat;
            return someResult.ToString();
        }
        
        // And many more thing to come
    }
}