using System.Collections.Generic;
using UnityEngine;

namespace ImportedScenes.PurlySnowman_A04_20260427_Copy
{
    public class InfiniteLevel : MonoBehaviour
    {
        // Store the player transform used to decide when to spawn and despawn chunks.
        public Transform player;

        // Store the platform sprites used for normal snowy ground.
        public Sprite[] platformSprites;

        // Store the rock sprites used for rocky bridge sections.
        public Sprite[] rockSprites;

        // Store the snow-gun sprite used for spawned cannons.
        public Sprite gunSprite;

        // Store the waterfall sprite used for spawned waterfalls.
        public Sprite waterSprite;

        // Store the pine-tree sprites used for scenery.
        public Sprite[] pineSprites;

        // Store the smaller scenery sprites used for mushrooms, signs, grass, and stumps.
        public Sprite[] decorSprites;

        // Store the snowball prefab used by spawned cannons.
        public GameObject snowballPrefab;

        // Store the animator controller used by spawned waterfalls.
        public RuntimeAnimatorController waterfallAnimator;

        // Store the ground layer to apply to generated ground blocks.
        public int groundLayer;

        // Store how far ahead of the player terrain should be spawned.
        public float spawnAheadDistance = 30f;

        // Store how far behind the player generated objects should be destroyed.
        public float despawnBehindDistance = 80f;

        // Store the spacing between spawned cannons.
        public float cannonSpacing = 20f;

        // Store the wall sprite used for the top and bottom level borders.
        public Sprite wallSprite;

        // Store the X position where the next chunk should start.
        private float nextChunkX;

        // Store the X position where the next cannon should spawn.
        private float nextCannonX;

        // Track generated scene objects so they can be cleaned up later.
        private readonly List<GameObject> spawned = new();

        // Store which side the next spawned cannon should face.
        private bool leftSideNext = true;

        // Define the chunk layouts the generator may choose from.
        private enum ChunkKind
        {
            LongFlat,
            Hill,
            Broken,
            Bridge,
            Waterfall,
            Elevated
        }

#if UNITY_EDITOR
        private void Awake()
        {
            // Store the asset folder that contains the snowy environment sprites.
            const string basePath = "Assets/HobiSoLoved/2D Platformer Snowy/Sprites/";

            // Auto-fill platform sprites in the editor when the array is empty.
            if (platformSprites == null || platformSprites.Length == 0)
            {
                platformSprites = LoadSprites(basePath + "Platform snow2.png", basePath + "Platform2.png", basePath + "Platform3.png");
            }

            // Auto-fill rock sprites in the editor when the array is empty.
            if (rockSprites == null || rockSprites.Length == 0)
            {
                rockSprites = LoadSprites(basePath + "Rock1.png", basePath + "Rock2.png");
            }

            // Auto-fill pine sprites in the editor when the array is empty.
            if (pineSprites == null || pineSprites.Length == 0)
            {
                pineSprites = LoadSprites(basePath + "Pine-Snow.png", basePath + "Pine snow2.png", basePath + "Pine snow3.png");
            }

            // Auto-fill decor sprites in the editor when the array is empty.
            if (decorSprites == null || decorSprites.Length == 0)
            {
                decorSprites = LoadSprites(basePath + "Mushroom.png", basePath + "Sign.png", basePath + "stump_snow.png", basePath + "Grass.png");
            }
        }

        private static Sprite[] LoadSprites(params string[] paths)
        {
            // Create a list to collect the sprites that were found.
            List<Sprite> sprites = new();

            // Visit each requested asset path.
            foreach (string path in paths)
            {
                // Try to load the sprite from the Unity editor asset database.
                Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);

                // Skip paths that do not resolve to a sprite.
                if (sprite == null)
                {
                    continue;
                }

                // Add the loaded sprite to the output list.
                sprites.Add(sprite);
            }

            // Return the loaded sprites or null when none were found.
            return sprites.Count > 0 ? sprites.ToArray() : null;
        }
#endif

        private void Start()
        {
            // Start chunk generation from this object's current X position.
            nextChunkX = transform.position.x;

            // Spawn the first cannon some distance ahead of the generator origin.
            nextCannonX = transform.position.x + cannonSpacing;

            // Pre-build several chunks so the level already exists at game start.
            for (int i = 0; i < 4; i++)
            {
                SpawnNextChunk();
            }
        }

        private void Update()
        {
            // Stop if the player reference is missing.
            if (player == null)
            {
                return;
            }

            // Keep spawning chunks while the player approaches the current edge.
            while (player.position.x + spawnAheadDistance > nextChunkX)
            {
                SpawnNextChunk();
            }

            // Keep spawning cannons while the player approaches the next cannon slot.
            while (player.position.x + 12f > nextCannonX)
            {
                SpawnCannon(nextCannonX);
                nextCannonX += cannonSpacing;
            }

            // Remove spawned objects that are far behind the player.
            for (int index = spawned.Count - 1; index >= 0; index--)
            {
                // Read the current tracked object.
                GameObject go = spawned[index];

                // Remove null entries created by prior destruction.
                if (go == null)
                {
                    spawned.RemoveAt(index);
                    continue;
                }

                // Skip objects that are still close enough to the player.
                if (go.transform.position.x >= player.position.x - despawnBehindDistance)
                {
                    continue;
                }

                // Destroy objects that are too far behind the player.
                Destroy(go);
                spawned.RemoveAt(index);
            }
        }

        private void SpawnNextChunk()
        {
            // Pick one random chunk layout.
            ChunkKind kind = (ChunkKind)Random.Range(0, 6);

            // Store the starting X position for this chunk.
            float startX = nextChunkX;

            // Pick a low base height most of the time, with some elevated cases.
            int baseY = Random.Range(0, 100) < 60 ? 0 : Random.Range(1, 3);

            // Build the selected chunk layout.
            switch (kind)
            {
                case ChunkKind.LongFlat:
                    MakeBlock(startX, 12, baseY, false);
                    AddDecorations(startX, 12, baseY);
                    break;

                case ChunkKind.Hill:
                    MakeBlock(startX, 12, baseY, false);
                    MakeBlock(startX + 4f, 4, baseY + 1, false);
                    MakeBlock(startX + 5f, 2, baseY + 2, false);
                    AddDecorations(startX, 4, baseY);
                    AddDecorations(startX + 8f, 4, baseY);
                    break;

                case ChunkKind.Broken:
                    MakeBlock(startX, 4, baseY, false);
                    MakeBlock(startX + 6f, 6, baseY, false);
                    AddDecorations(startX, 4, baseY);
                    AddDecorations(startX + 6f, 6, baseY);
                    break;

                case ChunkKind.Bridge:
                    MakeBlock(startX, 4, baseY, false);
                    MakeBlock(startX + 4f, 4, baseY, true);
                    MakeBlock(startX + 8f, 4, baseY, false);
                    AddDecorations(startX, 4, baseY);
                    AddDecorations(startX + 8f, 4, baseY);
                    break;

                case ChunkKind.Waterfall:
                    MakeBlock(startX, 4, baseY, false);
                    MakeBlock(startX + 4f, 4, baseY, true);
                    MakeBlock(startX + 8f, 4, baseY, false);
                    AddWaterfall(startX + 5.5f, baseY);
                    AddDecorations(startX, 4, baseY);
                    AddDecorations(startX + 8f, 4, baseY);
                    break;

                case ChunkKind.Elevated:
                    int elevatedY = baseY + 2;
                    MakeBlock(startX, 12, elevatedY, false);
                    AddDecorations(startX, 12, elevatedY);
                    break;
            }

            // Add the top and bottom wall strips for this chunk.
            AddWallStrips(startX, 12);

            // Advance the chunk cursor to the next chunk position.
            nextChunkX += 12f;
        }

        private void AddWallStrips(float startX, int width)
        {
            // Stop if there is no wall sprite assigned.
            if (wallSprite == null)
            {
                return;
            }

            // Spawn the upper wall strip.
            MakeWallStrip(startX, width, 8.5f);

            // Spawn the lower wall strip.
            MakeWallStrip(startX, width, -2.5f);
        }

        private void MakeWallStrip(float startX, int width, float y)
        {
            // Create the wall-strip object.
            GameObject go = new("InfWall");

            // Parent the strip under the level generator.
            go.transform.SetParent(transform);

            // Position the strip in the center of the chunk.
            go.transform.position = new Vector3(startX + width * 0.5f, y, 0f);

            // Scale the strip to match the chunk width.
            go.transform.localScale = new Vector3(width, 1f, 1f);

            // Add the sprite renderer for the wall strip.
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();

            // Apply the configured wall sprite.
            renderer.sprite = wallSprite;

            // Render the strip as a simple sprite.
            renderer.drawMode = SpriteDrawMode.Simple;

            // Push the strip behind gameplay objects.
            renderer.sortingOrder = -50;

            // Track the spawned object for later cleanup.
            spawned.Add(go);
        }

        private void MakeBlock(float startX, int width, int y, bool useRockSprites)
        {
            // Create the block object and pick its display name.
            GameObject block = new(useRockSprites ? "InfRock" : "InfBlock");

            // Parent the block under the level generator.
            block.transform.SetParent(transform);

            // Position the block so its collider sits at the intended height.
            block.transform.position = new Vector3(startX + width * 0.5f, y + 0.5f, 0f);

            // Tag the block as ground.
            block.tag = "Ground";

            // Put the block on the configured ground layer.
            block.layer = groundLayer;

            // Add one collider covering the full block width.
            BoxCollider2D collider = block.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(width, 1f);

            // Pick the sprite set for this block type.
            Sprite[] sprites = useRockSprites ? rockSprites : platformSprites;

            // Stop early when no sprites are available.
            if (sprites == null || sprites.Length == 0)
            {
                spawned.Add(block);
                return;
            }

            // Tile sprites from left to right across the block.
            for (float x = startX; x < startX + width;)
            {
                // Pick one random sprite from the requested set.
                Sprite sprite = PickRandom(sprites);

                // Skip null sprite entries safely.
                if (sprite == null)
                {
                    break;
                }

                // Read the natural sprite size.
                float spriteWidth = sprite.bounds.size.x > 0f ? sprite.bounds.size.x : 1f;
                float spriteHeight = sprite.bounds.size.y > 0f ? sprite.bounds.size.y : 1f;

                // Create one tile object.
                GameObject tile = new("Tile");

                // Parent the tile under the block root.
                tile.transform.SetParent(block.transform);

                // Align the tile so its top meets the walkable surface.
                tile.transform.position = new Vector3(x + spriteWidth * 0.5f, y + 1f - spriteHeight * 0.5f, 0f);

                // Add the tile sprite renderer.
                SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();

                // Apply the chosen sprite.
                renderer.sprite = sprite;

                // Render the tile as a simple sprite.
                renderer.drawMode = SpriteDrawMode.Simple;

                // Keep the tile on the default ground sorting layer.
                renderer.sortingOrder = 0;

                // Advance to the next tiling position.
                x += spriteWidth;
            }

            // Track the block for later cleanup.
            spawned.Add(block);
        }

        private void AddDecorations(float startX, int width, int baseY)
        {
            // Spawn a few trees when pine sprites are available.
            if (pineSprites != null && pineSprites.Length > 0)
            {
                for (int i = 0; i < Random.Range(1, 3); i++)
                {
                    SpawnDecorSprite("InfTree", PickRandom(pineSprites), startX, width, baseY, Random.Range(2f, 3f), 2);
                }
            }

            // Spawn a few smaller props when decor sprites are available.
            if (decorSprites != null && decorSprites.Length > 0 && width >= 2)
            {
                for (int i = 0; i < Random.Range(0, 2); i++)
                {
                    SpawnDecorSprite("InfDecor", PickRandom(decorSprites), startX, width, baseY, 0.8f, 2);
                }
            }

            // Add a few balloon spawn markers across the chunk.
            for (int slot = 0; slot < 3; slot++)
            {
                // Convert the slot index into a normalized position across the chunk.
                float t = (slot + 1f) / 4f;

                // Create the spawn marker object.
                GameObject marker = new("InfSpawn");

                // Parent the marker under the level generator.
                marker.transform.SetParent(transform);

                // Position the marker above the platform.
                marker.transform.position = new Vector3(
                    startX + width * t,
                    baseY + 1.5f + (slot % 2 == 0 ? 0f : 1.8f),
                    0f
                );

                // Mark the object as a balloon spawn point.
                marker.AddComponent<BalloonSpawnPoint>();

                // Track the marker for later cleanup.
                spawned.Add(marker);
            }
        }

        private void SpawnDecorSprite(string objectName, Sprite sprite, float startX, int width, int baseY, float targetHeight, int sortingOrder)
        {
            // Stop if the selected sprite is missing.
            if (sprite == null)
            {
                return;
            }

            // Pick one random X position within the chunk width.
            float x = startX + Random.Range(0.5f, width - 0.5f);

            // Read the sprite's natural height.
            float naturalHeight = sprite.bounds.size.y;

            // Convert the desired height into a scale multiplier.
            float scale = naturalHeight > 0f ? targetHeight / naturalHeight : 1f;

            // Create the decorative object.
            GameObject go = new(objectName);

            // Parent the decorative object under the level generator.
            go.transform.SetParent(transform);

            // Place the object so it rests on top of the platform.
            go.transform.position = new Vector3(x, baseY + 1f + targetHeight * 0.5f, 0f);

            // Scale the object to the requested display height.
            go.transform.localScale = new Vector3(scale, scale, 1f);

            // Add the sprite renderer.
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();

            // Apply the decorative sprite.
            renderer.sprite = sprite;

            // Render it as a simple sprite.
            renderer.drawMode = SpriteDrawMode.Simple;

            // Place it in front of the ground.
            renderer.sortingOrder = sortingOrder;

            // Track the decorative object for later cleanup.
            spawned.Add(go);
        }

        private void AddWaterfall(float worldX, int baseY)
        {
            // Stop if there is no waterfall sprite assigned.
            if (waterSprite == null)
            {
                return;
            }

            // Store the intended waterfall width.
            const float targetWidth = 1.5f;

            // Store the intended waterfall height.
            const float targetHeight = 8f;

            // Read the waterfall sprite's natural size.
            float naturalWidth = waterSprite.bounds.size.x;
            float naturalHeight = waterSprite.bounds.size.y;

            // Convert the desired width into a scale multiplier.
            float scaleX = naturalWidth > 0f ? targetWidth / naturalWidth : 1f;

            // Convert the desired height into a scale multiplier.
            float scaleY = naturalHeight > 0f ? targetHeight / naturalHeight : 1f;

            // Create the waterfall object.
            GameObject go = new("InfWaterfall");

            // Parent the waterfall under the level generator.
            go.transform.SetParent(transform);

            // Place the waterfall under the bridge section.
            go.transform.position = new Vector3(worldX, baseY - targetHeight * 0.5f, 0f);

            // Scale the waterfall to the requested size.
            go.transform.localScale = new Vector3(scaleX, scaleY, 1f);

            // Tag the object as a waterfall.
            go.tag = "Waterfall";

            // Add the waterfall sprite renderer.
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();

            // Apply the configured waterfall sprite.
            renderer.sprite = waterSprite;

            // Render it as a simple sprite.
            renderer.drawMode = SpriteDrawMode.Simple;

            // Draw it in front of ground but behind UI.
            renderer.sortingOrder = 4;

            // Add a trigger collider for waterfall death detection.
            BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = Vector2.one;

            // Add the waterfall gameplay behavior.
            go.AddComponent<Waterfall>();

            // Add animation only when a controller was assigned.
            if (waterfallAnimator != null)
            {
                go.AddComponent<Animator>().runtimeAnimatorController = waterfallAnimator;
            }

            // Track the waterfall for later cleanup.
            spawned.Add(go);
        }

        private void SpawnCannon(float worldX)
        {
            // Stop if required cannon assets are missing.
            if (gunSprite == null || snowballPrefab == null)
            {
                return;
            }

            // Choose the facing side for this cannon.
            int facing = leftSideNext ? 1 : -1;

            // Flip the side used by the next cannon.
            leftSideNext = !leftSideNext;

            // Read the cannon sprite's natural height.
            float naturalHeight = gunSprite.bounds.size.y;

            // Store the intended cannon height.
            float targetHeight = 1.4f;

            // Convert the desired height into a scale multiplier.
            float scale = naturalHeight > 0f ? targetHeight / naturalHeight : 1f;

            // Create the cannon root object.
            GameObject go = new("InfCannon");

            // Parent the cannon under the level generator.
            go.transform.SetParent(transform);

            // Position the cannon near the top of the scene.
            go.transform.position = new Vector3(worldX, 7f, 0f);

            // Flip and scale the cannon based on its facing side.
            go.transform.localScale = new Vector3(facing < 0 ? -scale : scale, scale, 1f);

            // Add the cannon sprite renderer.
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();

            // Apply the configured cannon sprite.
            renderer.sprite = gunSprite;

            // Render the cannon as a simple sprite.
            renderer.drawMode = SpriteDrawMode.Simple;

            // Draw the cannon above the level art.
            renderer.sortingOrder = 3;

            // Create the cannon muzzle child.
            GameObject muzzle = new("Muzzle");

            // Parent the muzzle under the cannon.
            muzzle.transform.SetParent(go.transform);

            // Place the muzzle at the front of the cannon sprite.
            muzzle.transform.localPosition = new Vector3(0.5f, 0f, 0f);

            // Add the cannon behavior script.
            SnowGun gun = go.AddComponent<SnowGun>();

            // Pass the runtime setup references into the cannon.
            gun.Configure(snowballPrefab, muzzle.transform, true);

            // Track the cannon for later cleanup.
            spawned.Add(go);
        }

        private static Sprite PickRandom(Sprite[] sprites)
        {
            // Return null when the sprite array is missing or empty.
            if (sprites == null || sprites.Length == 0)
            {
                return null;
            }

            // Return one random sprite from the array.
            return sprites[Random.Range(0, sprites.Length)];
        }
    }
}
