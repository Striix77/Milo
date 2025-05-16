using UnityEngine;
using UnityEngine.Tilemaps;

public class DigglyMovement : MonoBehaviour
{

    private DigglyAnimator digglyAnimator;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private Tilemap tilemap;

    void Start()
    {
        digglyAnimator = GetComponent<DigglyAnimator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        digglyAnimator.canTeleport = true;
        while (digglyAnimator.canTeleport)
        {
            TeleportToRandomTilemap();
        }
    }

    void Update()
    {
        if (digglyAnimator.canTeleport == true)
        {
            TeleportToRandomTilemap();
        }
    }

    private void TeleportToRandomTilemap()
    {

        BoundsInt bounds = tilemap.cellBounds;
        int x = Random.Range(bounds.x, bounds.xMax);
        int y = Random.Range(bounds.y, bounds.yMax);
        Vector3Int randomCell = new Vector3Int(x, y, 0);
        Vector3Int randomCellUnder = new Vector3Int(x, y - 1, 0);
        Debug.DrawLine(tilemap.GetCellCenterWorld(randomCell), tilemap.GetCellCenterWorld(randomCellUnder), Color.red, 2f);
        if (IsCellVisible(randomCell) && !tilemap.HasTile(randomCell) && tilemap.HasTile(randomCellUnder))
        {
            Vector3 worldPosition = tilemap.GetCellCenterWorld(randomCell);
            transform.position = worldPosition;
            digglyAnimator.canTeleport = false;
            digglyAnimator.PlayAppearAnimation();
            Debug.Log("Teleporting to random tilemap position.");
        }
    }

    private bool IsCellVisible(Vector3Int cellPosition)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(worldPosition);
        return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;
    }
}
