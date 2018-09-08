using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;

public class SimpleStoreHandler : MonoBehaviour {

    // Select an ABI from our project resources
    // We got these from the migration script in Truffle
    public TextAsset simpleStoreABI;
    public TextAsset simpleStoreAddress;
    
    private List<String> weapon_list = new List<String>();
    private Dictionary<String, int> my_weapon_list = new Dictionary<string, int>();
    
    // Use this for initialization
    public async void Start () {
        this.weapon_list.Add("A");
        this.weapon_list.Add("B");
        
        // Generate new keys for this user
        // TODO - Either store these or let the user enter a private key
        var privateKey = CryptoUtils.GeneratePrivateKey();
        var publicKey = CryptoUtils.PublicKeyFromPrivateKey(privateKey);

        // Get the contract
        var contract = await GetContract(privateKey, publicKey);
        contract.EventReceived += EventReceivedHandler;
        
        // Make a call
        await CallAcquireWeapon(contract, "A");
        await CallGetWeaponList(contract);
        await CallContractTest(contract);
        await StaticCallContract(contract);;
    }

    // Get's the contract as an object 
    async Task<EvmContract> GetContract(byte[] privateKey, byte[] publicKey)
    {   
        // Get the writer and reader for the Loom node
        var writer = RPCClientFactory.Configure()
            .WithLogger(Debug.unityLogger)
            .WithWebSocket("ws://127.0.0.1:46657/websocket")
            .Create();
        var reader = RPCClientFactory.Configure()
            .WithLogger(Debug.unityLogger)
            .WithWebSocket("ws://127.0.0.1:9999/queryws")
            .Create();

        // Create a client object from them
        var client = new DAppChainClient(writer, reader)
            { Logger = Debug.unityLogger };

        // required middleware
        client.TxMiddleware = new TxMiddleware(new ITxMiddlewareHandler[]
        {
            new NonceTxMiddleware(publicKey, client),
            new SignedTxMiddleware(privateKey)
        });

        // ABI of the Solidity contract
        string abi = simpleStoreABI.ToString();
        // Address of the Solidity contract
        var contractAddr = Address.FromHexString(simpleStoreAddress.ToString());
        // Address of the user
        var callerAddr = Address.FromPublicKey(publicKey);
        // Return the Contract object
        return new EvmContract(client, contractAddr, callerAddr, abi);
    }

    public async Task StaticCallContract(EvmContract contract)
    {
        if (contract == null)
        {
            throw new Exception("Not signed in!");
        }

        Debug.Log("Calling smart contract...");

        int result = await contract.StaticCallSimpleTypeOutputAsync<int>("get");
        if (result != null)
        {
            Debug.Log("Smart contract returned: " + result);
        } else
        {
            Debug.LogError("Smart contract didn't return anything!");
        }
    }

    private async Task CallAcquireWeapon(EvmContract contract, string weapon)
    {
        await contract.CallAsync("acquireWeapon", weapon);
    }
    
    private async Task CallGetWeaponList(EvmContract contract)
    {
        for (var i = 0; i < this.weapon_list.Count; i++) {
            Debug.Log(this.weapon_list[i]);
            var result = await contract.StaticCallSimpleTypeOutputAsync<int>("getWeaponList", this.weapon_list[i]);
            Debug.Log(result);
            if (result != null) this.my_weapon_list.Add(this.weapon_list[i], result);
        }
        
        foreach (KeyValuePair<string, int> weapon in this.my_weapon_list)
        {
            Debug.Log(weapon.Key + weapon.Value);
        }
    }
    
    /* Test Code get Event Result */
    private async Task CallContractTest(EvmContract contract)
    {
        if (contract == null)
        {
            throw new Exception("Not signed in!");
        }
        Debug.Log("Calling smart contract... testEvent");
        
        await contract.CallAsync("testEvent", 0);
    }
    private class OnTestEvent
    {
        [Parameter("string", "_state", 1)]
        public string State { get; set; }
        
        //[Parameter("uint", "_rank", 2)]
        //public BigInteger Rank { get; set; }
        
        //[Parameter("uint", "_sender", 3)]
        //public BigInteger Sender { get; set; }
    }
    
    private void EventReceivedHandler(object sender, EvmChainEventArgs e)
    {
        if (e.EventName == "GetTest") {
            OnTestEvent onTestEvent = e.DecodeEventDTO<OnTestEvent>();
            // JsonTileMapState jsonTileMapState = JsonUtility.FromJson<JsonTileMapState>(onTileMapStateUpdateEvent.State);
            Debug.Log(onTestEvent.State);
            //Debug.Log(onTestEvent.Rank);
            //Debug.Log(onTestEvent.Sender);
        }

    }
}