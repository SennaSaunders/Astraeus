namespace Code.ObserverPattern {
    public interface IIntObserver {
        public void UpdateSelf(int value);
        public bool UpdateNeeded(int value);
    }
}