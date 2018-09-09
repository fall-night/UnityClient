using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Assets.Scripts;
using UnityEngine;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LoadSceneHandler))]
public class SimpleStoreHandler : MonoBehaviour {

    // Select an ABI from our project resources
    // We got these from the migration script in Truffle
    [SerializeField] private TextAsset contractAbi;
    [SerializeField] private TextAsset contractAddress;
    private LoadSceneHandler _loadSceneHandler;
    
    private List<String> weapon_list = new List<String>();
    private Dictionary<String, int> my_weapon_list = new Dictionary<string, int>();
    private EvmContract contract;

    public void Awake()
    {
        this._loadSceneHandler = GetComponent<LoadSceneHandler>();
    }
    
    // Use this for initialization
    public async void Start () {
        var privateKey = ReadPrivateKeyFromPlayerPrefs("privateKey");
        var publicKey = CryptoUtils.PublicKeyFromPrivateKey(privateKey);

        // Get the contract
        this.contract = await GetContract(privateKey, publicKey);
        this.contract.EventReceived += EventReceivedHandler;
    }

    private static byte[] StringToByteArray(string str)
    {
        return System.Convert.FromBase64String(str);
    }
    
    private static byte[] ReadPrivateKeyFromPlayerPrefs (string tag)
    {
        string base64String = PlayerPrefs.GetString (tag, null);
        var key = StringToByteArray(base64String);
        return key;
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

        string abi = this.contractAbi.ToString();
        var contractAddr = Address.FromHexString(this.contractAddress.ToString());
        var callerAddr = Address.FromPublicKey(publicKey);
        return new EvmContract(client, contractAddr, callerAddr, abi);
    }

    public async void OnClickStartButton()
    {
        await CallStartButton(1);
    }

    private async Task CallStartButton(int state)
    {
        await this.contract.CallAsync("Ready", 1);
    }

    private class OnStartEvent
    {
        [Parameter("uint", "user1", 1)]
        public BigInteger User1 { get; set; }
        
        [Parameter("uint", "user2", 2)]
        public BigInteger User2 { get; set; }
        
//        [Parameter("uint", "user3", 3)]
//        public BigInteger User3 { get; set; }
    }
    
    private void EventReceivedHandler(object sender, EvmChainEventArgs e)
    {
        Debug.Log("Event");
        Debug.Log(e.EventName);
        if (e.EventName == "StartGame") {
            OnStartEvent onTestEvent = e.DecodeEventDTO<OnStartEvent>();
            // JsonTileMapState jsonTileMapState = JsonUtility.FromJson<JsonTileMapState>(onTileMapStateUpdateEvent.State);
            Debug.Log(onTestEvent.User1);
            Debug.Log(onTestEvent.User2);
            this._loadSceneHandler.Load("Scene/GameScene");
            // Debug.Log(onTestEvent.User3);
        }

    }
}