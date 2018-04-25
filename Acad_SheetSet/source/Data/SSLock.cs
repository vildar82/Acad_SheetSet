using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACSMCOMPONENTS20Lib;
using JetBrains.Annotations;

namespace Acad_SheetSet.Data
{
    public class SSLock : IDisposable
    {
        private readonly AcSmDatabase ssDb;

        public SSLock([NotNull] AcSmDatabase ssDb)
        {
            this.ssDb = ssDb;
            if (ssDb.GetLockStatus() == AcSmLockStatus.AcSmLockStatus_UnLocked)
            {
                ssDb.LockDb(ssDb);
            }
            throw new Exception("Подшивка заблокарована. Попробуйте позже.");
        }

        public void Dispose()
        {
            ssDb.UnlockDb(ssDb);
        }
    }
}
