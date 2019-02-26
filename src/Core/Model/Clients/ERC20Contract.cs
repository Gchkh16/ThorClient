using System;
using ThorClient.Core.Model.Clients.Base;

namespace ThorClient.Core.Model.Clients
{
    public class ERC20Contract : AbstractContract
    {
        private const string ERC20ABIString = 
            "[\n" +
            " {\n" +
            "  \"constant\": false,\n" +
            "  \"inputs\": [\n" +
            "   {\n" +
            "    \"name\": \"spender\",\n" +
            "    \"type\": \"address\"\n" +
            "   },\n" +
            "   {\n" +
            "    \"name\": \"value\",\n" +
            "    \"type\": \"uint256\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"name\": \"approve\",\n" +
            "  \"outputs\": [\n" +
            "   {\n" +
            "    \"name\": \"\",\n" +
            "    \"type\": \"bool\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"payable\": false,\n" +
            "  \"stateMutability\": \"nonpayable\",\n" +
            "  \"type\": \"function\"\n" +
            " },\n" +
            " {\n" +
            "  \"constant\": true,\n" +
            "  \"inputs\": [],\n" +
            "  \"name\": \"totalSupply\",\n" +
            "  \"outputs\": [\n" +
            "   {\n" +
            "    \"name\": \"\",\n" +
            "    \"type\": \"uint256\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"payable\": false,\n" +
            "  \"stateMutability\": \"view\",\n" +
            "  \"type\": \"function\"\n" +
            " },\n" +
            " {\n" +
            "  \"constant\": false,\n" +
            "  \"inputs\": [\n" +
            "   {\n" +
            "    \"name\": \"from\",\n" +
            "    \"type\": \"address\"\n" +
            "   },\n" +
            "   {\n" +
            "    \"name\": \"to\",\n" +
            "    \"type\": \"address\"\n" +
            "   },\n" +
            "   {\n" +
            "    \"name\": \"value\",\n" +
            "    \"type\": \"uint256\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"name\": \"transferFrom\",\n" +
            "  \"outputs\": [\n" +
            "   {\n" +
            "    \"name\": \"\",\n" +
            "    \"type\": \"bool\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"payable\": false,\n" +
            "  \"stateMutability\": \"nonpayable\",\n" +
            "  \"type\": \"function\"\n" +
            " },\n" +
            " {\n" +
            "  \"constant\": true,\n" +
            "  \"inputs\": [\n" +
            "   {\n" +
            "    \"name\": \"who\",\n" +
            "    \"type\": \"address\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"name\": \"balanceOf\",\n" +
            "  \"outputs\": [\n" +
            "   {\n" +
            "    \"name\": \"\",\n" +
            "    \"type\": \"uint256\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"payable\": false,\n" +
            "  \"stateMutability\": \"view\",\n" +
            "  \"type\": \"function\"\n" +
            " },\n" +
            " {\n" +
            "  \"constant\": false,\n" +
            "  \"inputs\": [\n" +
            "   {\n" +
            "    \"name\": \"to\",\n" +
            "    \"type\": \"address\"\n" +
            "   },\n" +
            "   {\n" +
            "    \"name\": \"value\",\n" +
            "    \"type\": \"uint256\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"name\": \"transfer\",\n" +
            "  \"outputs\": [\n" +
            "   {\n" +
            "    \"name\": \"\",\n" +
            "    \"type\": \"bool\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"payable\": false,\n" +
            "  \"stateMutability\": \"nonpayable\",\n" +
            "  \"type\": \"function\"\n" +
            " },\n" +
            " {\n" +
            "  \"constant\": true,\n" +
            "  \"inputs\": [\n" +
            "   {\n" +
            "    \"name\": \"owner\",\n" +
            "    \"type\": \"address\"\n" +
            "   },\n" +
            "   {\n" +
            "    \"name\": \"spender\",\n" +
            "    \"type\": \"address\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"name\": \"allowance\",\n" +
            "  \"outputs\": [\n" +
            "   {\n" +
            "    \"name\": \"\",\n" +
            "    \"type\": \"uint256\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"payable\": false,\n" +
            "  \"stateMutability\": \"view\",\n" +
            "  \"type\": \"function\"\n" +
            " },\n" +
            " {\n" +
            "  \"anonymous\": false,\n" +
            "  \"inputs\": [\n" +
            "   {\n" +
            "    \"indexed\": true,\n" +
            "    \"name\": \"owner\",\n" +
            "    \"type\": \"address\"\n" +
            "   },\n" +
            "   {\n" +
            "    \"indexed\": true,\n" +
            "    \"name\": \"spender\",\n" +
            "    \"type\": \"address\"\n" +
            "   },\n" +
            "   {\n" +
            "    \"indexed\": false,\n" +
            "    \"name\": \"value\",\n" +
            "    \"type\": \"uint256\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"name\": \"Approval\",\n" +
            "  \"type\": \"event\"\n" +
            " },\n" +
            " {\n" +
            "  \"anonymous\": false,\n" +
            "  \"inputs\": [\n" +
            "   {\n" +
            "    \"indexed\": true,\n" +
            "    \"name\": \"from\",\n" +
            "    \"type\": \"address\"\n" +
            "   },\n" +
            "   {\n" +
            "    \"indexed\": true,\n" +
            "    \"name\": \"to\",\n" +
            "    \"type\": \"address\"\n" +
            "   },\n" +
            "   {\n" +
            "    \"indexed\": false,\n" +
            "    \"name\": \"value\",\n" +
            "    \"type\": \"uint256\"\n" +
            "   }\n" +
            "  ],\n" +
            "  \"name\": \"Transfer\",\n" +
            "  \"type\": \"event\"\n" +
            " }\n" +
            "]";

        public ERC20Contract() : base(ERC20ABIString)
        {
            throw new NotImplementedException();
        }

    }
}
