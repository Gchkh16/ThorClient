using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using ThorClient.Core.Model.Exception;
using ThorClient.Utils;

namespace ThorClient.Core.Model.Clients.Base
{
    public class AbstractContract
    {
        private List<AbiDefinition> abiDefinitionList;

        public AbstractContract(string abiJsonString)
        {
            this.abiDefinitionList = ParseAbiList(abiJsonString);
        }

        public static ContractCall BuildCall(AbiDefinition definition, params object[] hexParameters)
        {
            return BuildCall(null, definition, hexParameters);
        }

        public static ContractCall BuildCall(string caller, AbiDefinition definition, params object[] hexParameters)
        {
            if (definition == null)
            {
                throw new InvalidArgumentException("definition is null");
            }
            string data = BuildData(definition, hexParameters);
            var contractCall = new ContractCall();
            contractCall.Data = data;
            contractCall.Value = "0x0";
            contractCall.Caller = caller;
            return contractCall;
        }

        protected static string BuildData(AbiDefinition abiDefinition, params object[] parameters)
        {
            if (abiDefinition == null)
            {
                return null;
            }
            int index;
            var inputs = abiDefinition.Inputs;
            if (inputs == null || parameters == null || inputs.Count != parameters.Length)
            {
                throw new InvalidArgumentException("Parameters length is not valid");
            }
            var dataBuffer = new StringBuilder();
            dataBuffer.Append(abiDefinition.GetHexMethodCodeNoPrefix());

            int dynamicDataOffset = ContractParamEncoder.GetLength(inputs, parameters) * 32;
            var dynamicData = new StringBuilder();

            for (index = 0; index < parameters.Length; index++)
            {
                var param = parameters[index];
                string abiType = inputs[index].Type;
                string code = ContractParamEncoder.Encode(abiType, param);
                bool isDynamic = ContractParamEncoder.IsDynamic(index, param, abiType);
                if (isDynamic)
                {
                    string encodedDataOffset = ContractParamEncoder.EncodeNumeric(BigInteger.ValueOf(dynamicDataOffset),
                        true);
                    dataBuffer.Append(encodedDataOffset);
                    dynamicData.Append(code);
                    dynamicDataOffset += code.Length >> 1;
                }
                else
                {
                    dataBuffer.Append(code);
                }
            }
            dataBuffer.Append(dynamicData);
            return "0x" + dataBuffer;
        }

        private static List<AbiDefinition> ParseAbiList(string abisString)
        {
            List<AbiDefinition> list = null;
            try
            {
                list = JsonConvert.DeserializeObject<List<AbiDefinition>>(abisString);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            if (list == null)
            {
                return null;
            }
            return list;
        }


        public AbiDefinition FindAbiDefinition(string name)
        {
            return FindAbiDefinition(name, "function", null);
        }

        public AbiDefinition FindAbiDefinition(string name, string type, List<string> inputTypes)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
            {
                throw new InvalidArgumentException("name or type is blank.");
            }
            foreach (var abiDefinition in abiDefinitionList)
            {
                if (abiDefinition.Name.Equals(name) && abiDefinition.Type.Equals(type))
                {
                    if (inputTypes != null)
                    {
                        if (CheckInputsType(inputTypes, abiDefinition))
                        {
                            return abiDefinition;
                        }
                    }
                    else
                    { // find abi by name ignore the arguments.
                        return abiDefinition;
                    }
                }
            }
            throw new InvalidArgumentException(this.GetType().Name + " doesn't has abi define of name:" + name);
        }

        private bool CheckInputsType(List<string> inputTypes, AbiDefinition abiDefinition)
        {
            if (inputTypes == null)
            {
                return false;
            }
            if (abiDefinition == null)
            {
                return false;
            }
            if (inputTypes.Count != abiDefinition.Inputs.Count)
            {
                return false;
            }
            for (int index = 0; index < inputTypes.Count; index++)
            {
                if (!inputTypes[index].Equals(abiDefinition.Inputs[index].Type))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// build transaction clause
        /// </summary>
        /// <param name="toAddress"><seealso cref="Address"/> </param>
        /// <param name="abiDefinition"><seealso cref="AbiDefinition"/> Abi definition. </param>
        /// <param name="hexArguments"><seealso cref="String"/> </param>
        /// <returns> <seealso cref="ToClause"/> </returns>
        public static ToClause BuildToClause(Address toAddress, AbiDefinition abiDefinition, params object[] hexArguments)
        {
            ToData toData = new ToData();
            String data = BuildData(abiDefinition, hexArguments);
            toData.SetData(data);
            return new ToClause(toAddress, Amount.ZERO, toData);
        }
    }
}
