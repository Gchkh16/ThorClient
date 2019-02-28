using System;

namespace ThorClient.Utils
{
    public static class Assertions
    {
        /// <summary>
        /// Throws Exception is passed assertion not true
        /// </summary>
        /// <param name="assertionResult">condition result</param>
        /// <param name="errorMessage">exception message to throw</param>
        public static void VerifyPrecondition(bool assertionResult, string errorMessage)
        {
            if (!assertionResult)
            {
                throw new Exception(errorMessage);
            }
        }
    }
}
