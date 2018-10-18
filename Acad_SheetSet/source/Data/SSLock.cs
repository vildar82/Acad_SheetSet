﻿namespace Acad_SheetSet.Data
{
    using System;
#if v2016
    using ACSMCOMPONENTS20Lib;
#elif v2017
    using ACSMCOMPONENTS21Lib;
#elif v2018
    using ACSMCOMPONENTS22Lib;
#endif
    using JetBrains.Annotations;

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