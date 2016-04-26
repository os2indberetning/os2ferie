using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.KMDVacationService.Interfaces
{
    public interface IReportAbsence
    {
        KMD_FerieService.BAPIRET2 Set_Absence_Plus(string iv_begda, string iv_begda_old, string iv_begti,
            string iv_begti_old, string iv_endda, string iv_endda_old, string iv_endti, string iv_endti_old,
            string iv_extra_data, string iv_opera, string iv_pernr, string iv_subty, string iv_subty_old);
    }
}
