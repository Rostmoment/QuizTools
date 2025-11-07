using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace QuizTools.Vseosvita
{
    class VseosvitaConstants
    {
        public const string BASE_URL = "https://vseosvita.ua/";
        public const string CREATE_SESSION_FOR_TEST_URL = "https://vseosvita.ua/test/start/{0}";
        public const string JOIN_TEST_URL = "https://vseosvita.ua/test/go-settings?code={0}";
        public const string GO_OPL_URL = "https://vseosvita.ua/test/go-olp";
        public const string ACTIVE_SCREEN_DATA_URL = "https://vseosvita.ua/ext/test-designer/testing-pupil/active-screen-data?isAjax=1&isAjaxUrl=https%3A%2F%2Fvseosvita.ua%2Ftest%2Fgo-olp&user_key={0}";
        public const string START_EXECUTION_URL = "https://vseosvita.ua/ext/test-designer/testing-pupil/start-execution?isAjaxUrl=https%3A%2F%2Fvseosvita.ua%2Ftest%2Fgo-olp&user_key={0}";
        public const string END_TEST_URL = "https://vseosvita.ua/ext/test-designer/testing-pupil/finish-test?isAjax=1&isAjaxUrl=https%3A%2F%2Fvseosvita.ua%2Ftest%2Fgo-olp&user_key={0}";
    }
}
