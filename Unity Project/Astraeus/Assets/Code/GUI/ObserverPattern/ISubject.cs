namespace Code.GUI.ObserverPattern {
    public interface ISubject<T> {
        public void AddObserver(T observer);
        public void RemoveObserver(T observer);
        public void ClearObservers();
        public void NotifyObservers();
    }
}