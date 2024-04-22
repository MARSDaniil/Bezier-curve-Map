using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using GHeart;

public class GameInstance :MonoBehaviour {
    public GameInstance Instance { get; private set; }

    [SerializeField] private GameObject m_loader;
    [SerializeField] private Transform m_mapPosition;
    private MapController m_mapController;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        MapInitTask();
    }
 
    private async UniTask MapInitTask() {
        if (m_loader && !m_loader.gameObject.activeSelf) {
            m_loader.gameObject.SetActive(true);
        }
        await InitMap();
        m_loader?.SetActive(false);
        m_mapController?.MovePlayer();
    }

    private async UniTask InitMap() {
        GameObject map = await Addressables.InstantiateAsync(Constants.Addrasables.MAP_ADDRESS);
        if (map) {
            map.transform.position = m_mapPosition.position;
            m_mapController = map.GetComponent<MapController>();
            if (m_mapController) {
                m_mapController.SpawnMap();
            }
        }
    }
}
