using System;
using ThorClient.Clients.Base;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Core.Model.Clients.Base;
using ThorClient.Core.Model.Exception;
using ThorClient.Utils;
using ThorClient.Utils.Crypto;

namespace ThorClient.Clients
{
    /// <summary>
    /// ProtoType Contract is a native contract for user to do multiple parties payment(mpp).
    /// <a href="https://github.com/vechain/thor/wiki/Prototype(CN)">Prototype</a>
    /// <br>What is ProtoType for?</br>
    /// 
    /// In common scenario, if a sender A want to make a transaction to target B,
    /// the sender A need to pay the transaction fee(gas).
    /// 
    /// However, for some realistic reason, the target A want to pay the transaction fee. Then the mpp can do such a thing.
    /// The sender A can be a user of target B(<seealso cref="#addUsers(Address[], Address[], int, byte, int, ECKeyPair)"/>),
    /// then the target B can set user plan (credit and recovery) for all senders.
    /// After all the things is done, then the sender A do transaction to target B, if the fee is less than the credit,
    /// the ProtoType native contract is going to book fee(gas) from target B's account.
    /// 
    /// <br>How to use ProtoType?</br>
    /// First, you must be the master of the to address. By call <seealso cref="SetMasterAddress"/>, then you can query the master by
    /// <seealso cref="GetMasterAddress(Address, Revision)"/>
    /// 
    /// Second, you as a Master, you can set add user to user plan. This step is to set candidate, the one you want to give
    /// credit. <seealso cref="AddUsers(Address[], Address[], int, byte, int, ECKeyPair)"/>
    /// 
    /// Third, set a user plan to target address for all users on the users list.
    /// <seealso cref="SetCreditPlans(Address[], Amount[], Amount[], int, byte, int, ECKeyPair)"/>
    /// 
    /// 
    /// </summary>
    public class ProtoTypeContractClient : TransactionClient
    {
        /// <summary>
        /// Get a master address from target address. </summary>
        /// <param name="target"> required <seealso cref="Address"/> target address, means transfer to address. </param>
        /// <param name="revision"> optional can be null <seealso cref="Revision"/> block revision. </param>
        /// <returns> Contract call result <seealso cref="Revision"/> </returns>
        /// <exception cref="ClientIOException"> </exception>
        public static ContractCallResult GetMasterAddress(Address target, Revision revision)
        {

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "target is null");
            }
            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("master");
            if (abi == null)
            {
                throw new ArgumentNullException(nameof(abi), "Can not find abi master method");
            }
            ContractCall call = AbstractContract.BuildCall(abi, target.ToHexString(Prefix.ZeroLowerX));

            return CallContract(call, ProtoTypeContract.ContractAddress, revision);
        }


        /// <summary>
        /// Set master user, the caller must be the owner or master of target addresses. </summary>
        /// <param name="targets"> <seealso cref="Address"/> arrays as target address. </param>
        /// <param name="newMasters"> <seealso cref="Address"/> arrays as new master address. </param>
        /// <param name="gas">  contract invoker gas </param>
        /// <param name="gasCoef"> gas coef </param>
        /// <param name="expiration"> expiration, suggest 720 </param>
        /// <param name="keyPair"> private key <seealso cref="ECKeyPair"/> </param>
        /// <returns> <seealso cref="TransferResult"/> </returns>
        /// <exception cref="ClientIOException"> network error. </exception>
        public static TransferResult SetMasterAddress(Address[] targets, Address[] newMasters, int gas, byte gasCoef, int expiration, ECKeyPair keyPair)
        {
            if (targets == null)
            {
                throw ClientArgumentException.Exception("targets is null");
            }
            if (newMasters == null)
            {
                throw ClientArgumentException.Exception("newMasters in null");
            }
            if (targets.Length != newMasters.Length)
            {
                throw ClientArgumentException.Exception("targets size must equal to newMasters size. ");
            }

            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("setMaster");
            if (abi == null)
            {
                throw new Exception("Can not find abi master method");
            }
            ToClause[] clauses = new ToClause[targets.Length];
            for (int index = 0; index < targets.Length; index++)
            {
                clauses[index] = ProtoTypeContract.BuildToClause(ProtoTypeContract.ContractAddress, abi, targets[index].ToHexString(Prefix.ZeroLowerX), newMasters[index].ToHexString(Prefix.ZeroLowerX));

            }
            return InvokeContractMethod(clauses, gas, gasCoef, expiration, keyPair);
        }

        /// <summary>
        /// Add user to user plan, the caller must be the owner or master of target addresses. </summary>
        /// <param name="targets"> <seealso cref="Address"/> arrays. </param>
        /// <param name="users"> <seealso cref="Address"/> arrays. </param>
        /// <param name="gas">  a contract invoker gas </param>
        /// <param name="gasCoef"> gas coef </param>
        /// <param name="expiration"> expiration, suggest it is 720. </param>
        /// <param name="keyPair"> <seealso cref="ECKeyPair"/> </param>
        /// <returns> <seealso cref="TransferResult"/> </returns>
        /// <exception cref="ClientIOException"> network error. </exception>
        public static TransferResult AddUsers(Address[] targets, Address[] users, int gas, byte gasCoef, int expiration, ECKeyPair keyPair)
        {
            if (targets == null)
            {
                throw ClientArgumentException.Exception("targets is null");
            }
            if (users == null)
            {
                throw ClientArgumentException.Exception("users is null");
            }
            if (targets.Length != users.Length)
            {
                throw ClientArgumentException.Exception("targets size must equal to users size. ");
            }
            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("addUser");
            if (abi == null)
            {
                throw new Exception("Can not find abi master method");
            }
            ToClause[] clauses = new ToClause[targets.Length];
            for (int index = 0; index < targets.Length; index++)
            {
                clauses[index] = ProtoTypeContract.BuildToClause(
                    ProtoTypeContract.ContractAddress,
                    abi,
                    targets[index].ToHexString(Prefix.ZeroLowerX),
                    users[index].ToHexString(Prefix.ZeroLowerX));

            }
            return InvokeContractMethod(clauses, gas, gasCoef, expiration, keyPair);

        }

        /// <summary>
        /// Remove user from target addresses, the caller must be the owner or master of target addresses. </summary>
        /// <param name="targets"> <seealso cref="Address"/> arrays. </param>
        /// <param name="users"> <seealso cref="Address"/> arrays. </param>
        /// <param name="gas">  a contract invoker gas </param>
        /// <param name="gasCoef"> gas coef </param>
        /// <param name="expiration"> expiration, suggest it is 720. </param>
        /// <param name="keyPair"> <seealso cref="ECKeyPair"/> </param>
        /// <returns> <seealso cref="TransferResult"/> </returns>
        /// <exception cref="ClientIOException"> network error. </exception>
        public static TransferResult RemoveUsers(Address[] targets, Address[] users, int gas, byte gasCoef, int expiration, ECKeyPair keyPair)
        {
            if (targets == null)
            {
                throw ClientArgumentException.Exception($"{nameof(targets)} is null");
            }
            if (users == null)
            {
                throw ClientArgumentException.Exception($"{nameof(users)} is null");
            }
            if (targets.Length != users.Length)
            {
                throw ClientArgumentException.Exception($"{nameof(targets)} size must equal to users size. ");
            }
            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("removeUser");
            if (abi == null)
            {
                throw new Exception("Can not find abi master method");
            }
            ToClause[] clauses = new ToClause[targets.Length];
            for (int index = 0; index < targets.Length; index++)
            {
                clauses[index] = ProtoTypeContract.BuildToClause(
                    ProtoTypeContract.ContractAddress,
                    abi,
                    targets[index].ToHexString(Prefix.ZeroLowerX),
                    users[index].ToHexString(Prefix.ZeroLowerX));

            }
            return InvokeContractMethod(clauses, gas, gasCoef, expiration, keyPair);
        }

        /// <summary>
        /// Set user plan, the caller must be the owner or master of target addresses. </summary>
        /// <param name="targets"> <seealso cref="Address"/> array. </param>
        /// <param name="credits"> <seealso cref="Amount"/> array. </param>
        /// <param name="recoveryRates"> <seealso cref="Amount"/> array. thor per seconds. </param>
        /// <returns> <seealso cref="TransferResult"/> </returns>
        /// <exception cref="ClientIOException"> network error. </exception>
        public static TransferResult SetCreditPlans(Address[] targets, Amount[] credits, Amount[] recoveryRates,
            int gas, byte gasCoef, int expiration, ECKeyPair keyPair)
        {

            if (targets == null)
            {
                throw ClientArgumentException.Exception($"{nameof(targets)} is null");
            }
            if (credits == null)
            {
                throw ClientArgumentException.Exception($"{nameof(credits)} is null");
            }
            if (recoveryRates == null)
            {
                throw ClientArgumentException.Exception($"{nameof(recoveryRates)} is null");
            }
            if (targets.Length != credits.Length || targets.Length != recoveryRates.Length)
            {
                throw ClientArgumentException.Exception("users.length must equal to credits.length and equal to recoveries.length");
            }
            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("setCreditPlan");
            if (abi == null)
            {
                throw new Exception("Can not find abi master method");
            }
            ToClause[] clauses = new ToClause[targets.Length];
            for (int index = 0; index < targets.Length; index++)
            {
                clauses[index] = ProtoTypeContract.BuildToClause(
                    ProtoTypeContract.ContractAddress,
                    abi,
                    targets[index].ToHexString(Prefix.ZeroLowerX),
                    credits[index].ToBigInteger(),
                    recoveryRates[index].ToBigInteger());

            }
            return InvokeContractMethod(clauses, gas, gasCoef, expiration, keyPair);
        }


        /// <summary>
        /// Check if user address is a user of target address. </summary>
        /// <param name="target"> required <seealso cref="Address"/> the target address. </param>
        /// <param name="user"> required <seealso cref="Address"/> the user address. </param>
        /// <param name="revision"> optional <seealso cref="Revision"/>. </param>
        /// <returns> <seealso cref="ContractCallResult"/> </returns>
        /// <exception cref="ClientIOException"> network error. </exception>
        public static ContractCallResult IsUser(Address target, Address user, Revision revision)
        {
            if (target == null)
            {
                throw ClientArgumentException.Exception($"{nameof(target)} address is null");
            }
            if (user == null)
            {
                throw ClientArgumentException.Exception($"{nameof(user)} address is null");
            }
            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("isUser");
            ContractCall call = AbstractContract.BuildCall(abi,
                    target.ToHexString(Prefix.ZeroLowerX),
                    user.ToHexString(Prefix.ZeroLowerX));

            return CallContract(call, ProtoTypeContract.ContractAddress, revision);
        }


        /// <summary>
        /// Get user plan </summary>
        /// <param name="target"> required <seealso cref="Address"/> </param>
        /// <param name="revision"> optional </param>
        /// <returns> <seealso cref="ContractCallResult"/> </returns>
        /// <exception cref="ClientIOException"> network error. </exception>
        public static ContractCallResult GetCreditPlan(Address target, Revision revision)
        {
            if (target == null)
            {
                throw ClientArgumentException.Exception($"{nameof(target)} is null");
            }
            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("creditPlan");
            ContractCall call = AbstractContract.BuildCall(abi,
                    target.ToHexString(Prefix.ZeroLowerX));

            return CallContract(call, ProtoTypeContract.ContractAddress, revision);
        }

        /// <summary>
        /// Get user credit from target address by some block revision. </summary>
        /// <param name="target"> <seealso cref="Address"/> target address. </param>
        /// <param name="user"> <seealso cref="Address"/> user address. </param>
        /// <param name="revision"> <seealso cref="Revision"/> revision. </param>
        /// <returns> <seealso cref="ContractCallResult"/> </returns>
        /// <exception cref="ClientIOException"> network error. </exception>
        public static ContractCallResult GetUserCredit(Address target, Address user, Revision revision)
        {
            if (target == null)
            {
                throw ClientArgumentException.Exception($"{nameof(target)} address is null");
            }
            if (user == null)
            {
                throw ClientArgumentException.Exception($"{nameof(user)} address is null");
            }

            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("userCredit");
            ContractCall call = AbstractContract.BuildCall(abi,
                    target.ToHexString(Prefix.ZeroLowerX),
                    user.ToHexString(Prefix.ZeroLowerX));

            return CallContract(call, ProtoTypeContract.ContractAddress, revision);
        }

        /// <summary>
        /// Sponsor the address, any address can sponsor target addresses. </summary>
        /// <param name="targets"> required <seealso cref="Address"/> the targets address </param>
        /// <param name="gas"> required int gas </param>
        /// <param name="gasCoef"> required byte gas coef </param>
        /// <param name="expiration"> required int expiration </param>
        /// <param name="keyPair"> required <seealso cref="Address"/> </param>
        /// <returns> <seealso cref="TransferResult"/> </returns>
        /// <exception cref="ClientIOException"> network error. </exception>
        public static TransferResult Sponsor(Address[] targets, int gas, byte gasCoef, int expiration, ECKeyPair keyPair)
        {
            if (targets == null)
            {
                throw ClientArgumentException.Exception($"{nameof(targets)} is null");
            }
            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("sponsor");
            if (abi == null)
            {
                throw new Exception("Can not find abi master method");
            }
            ToClause[] clauses = new ToClause[targets.Length];

            for (int index = 0; index < targets.Length; index++)
            {
                clauses[index] = ProtoTypeContract.BuildToClause(
                    ProtoTypeContract.ContractAddress,
                    abi,
                    targets[index].ToHexString(Prefix.ZeroLowerX)

                );
            }
            return InvokeContractMethod(clauses, gas, gasCoef, expiration, keyPair);
        }

        /// <summary>
        /// Unsponsor the address, only sponsor can invoke this method. </summary>
        /// <param name="targets"> required <seealso cref="Address"/> the targets address </param>
        /// <param name="gas"> required int gas </param>
        /// <param name="gasCoef"> required byte gas coef </param>
        /// <param name="expiration"> required int expiration </param>
        /// <param name="keyPair"> required <seealso cref="Address"/> </param>
        /// <returns> <seealso cref="TransferResult"/> </returns>
        /// <exception cref="ClientIOException"> network error. </exception>
        public static TransferResult Unsponsor(Address[] targets, int gas, byte gasCoef, int expiration, ECKeyPair keyPair)
        {
            if (targets == null)
            {
                throw ClientArgumentException.Exception($"{nameof(targets)} is null");
            }
            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("unsponsor");
            if (abi == null)
            {
                throw new Exception("Can not find abi master method");
            }
            ToClause[] clauses = new ToClause[targets.Length];

            for (int index = 0; index < targets.Length; index++)
            {
                clauses[index] = ProtoTypeContract.BuildToClause(
                    ProtoTypeContract.ContractAddress,
                    abi,
                    targets[index].ToHexString(Prefix.ZeroLowerX)
                );
            }
            return InvokeContractMethod(clauses, gas, gasCoef, expiration, keyPair);
        }

        /// <summary>
        /// Select sponsors for targets addresses, the caller must be the owner or master of target addresses. </summary>
        /// <param name="targets"> required <seealso cref="Address"/> array </param>
        /// <param name="sponsors"> required <seealso cref="Address"/> array </param>
        /// <param name="gas"> required int gas </param>
        /// <param name="gasCoef"> required byte gasCoef </param>
        /// <param name="expiration"> required int recommendation value is 720 </param>
        /// <param name="keyPair"> required <seealso cref="ECKeyPair"/> </param>
        /// <returns> <seealso cref="TransferResult"/>
        /// throw ClientIOException network error. </returns>
        public static TransferResult SelectSponsor(Address[] targets, Address[] sponsors, int gas, byte gasCoef, int expiration, ECKeyPair keyPair)
        {
            if (targets == null)
            {
                throw ClientArgumentException.Exception($"{nameof(targets)} is null");
            }
            if (sponsors == null)
            {
                throw ClientArgumentException.Exception($"{nameof(sponsors)} is null");
            }

            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("selectSponsor");
            if (abi == null)
            {
                throw new Exception("Can not find abi master method");
            }
            ToClause[] clauses = new ToClause[targets.Length];

            for (int index = 0; index < targets.Length; index++)
            {
                clauses[index] = ProtoTypeContract.BuildToClause(
                    ProtoTypeContract.ContractAddress,
                    abi,
                    targets[index].ToHexString(Prefix.ZeroLowerX),
                    sponsors[index].ToHexString(Prefix.ZeroLowerX)
                );
            }
            return InvokeContractMethod(clauses, gas, gasCoef, expiration, keyPair);
        }


        /// <summary>
        /// Get current sponsor from target address. </summary>
        /// <param name="target"> <seealso cref="Address"/> </param>
        /// <param name="revision"> <seealso cref="Revision"/> </param>
        /// <returns> <seealso cref="ContractCallResult"/>
        /// throw ClientIOException network error. </returns>
        public static ContractCallResult GetCurrentSponsor(Address target, Revision revision)
        {
            if (target == null)
            {
                throw ClientArgumentException.Exception("target is null");
            }
            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("currentSponsor");
            ContractCall call = AbstractContract.BuildCall(abi,
                    target.ToHexString(Prefix.ZeroLowerX));

            return CallContract(call, ProtoTypeContract.ContractAddress, revision);
        }

        /// <summary>
        /// Get boolean value if the sponsor address sponsored the target address. </summary>
        /// <param name="target"> required <seealso cref="Address"/> target address. </param>
        /// <param name="sponsor"> required <seealso cref="Address"/> sponsor </param>
        /// <param name="revision"> optional <seealso cref="Revision"/> block revision </param>
        /// <returns> <seealso cref="ContractCallResult"/>
        /// throw ClientIOException network error. </returns>
        public static ContractCallResult IsSponsor(Address target, Address sponsor, Revision revision)
        {
            if (target == null)
            {
                throw ClientArgumentException.Exception($"{nameof(target)} is null");
            }
            if (sponsor == null)
            {
                throw ClientArgumentException.Exception("sponsor is null");
            }
            AbiDefinition abi = ProtoTypeContract.DefaultContract.FindAbiDefinition("isSponsor");
            ContractCall call = AbstractContract.BuildCall(abi,
                    target.ToHexString(Prefix.ZeroLowerX),
                    sponsor.ToHexString(Prefix.ZeroLowerX));

            return CallContract(call, ProtoTypeContract.ContractAddress, revision);
        }
    }
}
