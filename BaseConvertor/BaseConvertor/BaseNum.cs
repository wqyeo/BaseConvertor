namespace BaseConvertor {
    [System.Serializable]
    public struct BaseNum {

        public int Base {
            get; private set;
        }

        public string Value {
            get; private set;
        }

        public BaseNum(int valueBase, string value) {
            Base = valueBase;
            Value = value;
        }
    }
}
