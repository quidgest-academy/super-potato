using CSGenio.persistence;

namespace DbAdmin.IntegrationTest
{
    public class DatabaseUtils
    {
        public static PersistentSupport GetUnexistingDatabase()
        {
            var currentDataSystem = CSGenio.framework.Configuration.DataSystems[0];
            string name = "NullDb";
            var dataSystem = currentDataSystem.ShallowCopy();
            dataSystem.Name = name;
            dataSystem.Schemas[0].Schema = name;

            CSGenio.framework.Configuration.DataSystems.Add(dataSystem);
            var sp = PersistentSupport.getPersistentSupport(name);
            CSGenio.framework.Configuration.DataSystems.Remove(dataSystem);
            return sp;
        }
    }
}