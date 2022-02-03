namespace BaseConvertor {
    [System.Serializable]
    public struct BaseNum {

        public int Base {
            get; private set;
        }

        public string Value {
            get; private set;
        }

        public BaseNum(string value, int valueBase = 10) {
            Base = valueBase;
            Value = value;
        }
    }
}
