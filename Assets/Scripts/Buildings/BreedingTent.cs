namespace Buildings
{
    public class BreedingTent : Building
    {
        private Caveman _secondCaveman;

        public bool AssignSecondCaveman(Caveman caveman)
        {
            if (_secondCaveman != null) return false;
            _secondCaveman = caveman;
            return true;
        }

        protected override void OnProductionComplete()
        {
            if (_secondCaveman == null) return;
            GodManager.Instance.SpawnBaby(transform.position);
        }
    }
}
