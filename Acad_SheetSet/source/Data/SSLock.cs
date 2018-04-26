// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 16:26

using System;
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
                ssDb.LockDb(ssDb);
            else
            {
                ssDb.GetLockOwnerInfo(out var user, out var machine);
                throw new Exception($"Подшивка заблокарована {user}, {machine}. Попробуйте позже.");
            }
        }

        public void Dispose()
        {
            ssDb.UnlockDb(ssDb);
        }
    }
}