# CV Warehouse

A browser game made in Unity 2D. Robots search a procedurally generated warehouse, take fragments of a CV out of boxes and carry them to a truck, while the CV assembles itself on the waybill. The full design is in [Docs/GameDesign.md](Docs/GameDesign.md).

## Testing a WebGL build locally

A WebGL build does not run when `index.html` is opened straight from disk, because the browser blocks loading over `file://`. Serve the build folder over HTTP instead. Python's built-in server is enough:

```bash
cd Builds
python -m http.server 8000 --bind 127.0.0.1
```

Open http://127.0.0.1:8000/ and stop the server with `Ctrl+C`.

- **Port in use:** pick another port, for example `8080`, and change the URL to match.
- **New build:** reload with `Ctrl+Shift+R` so the browser does not keep the old `.wasm` from its cache.
- **Phone on the same Wi-Fi:** use `--bind 0.0.0.0`, find the PC's address with `ipconfig` and open `http://<that-address>:8000/` on the phone. Allow Python through the firewall on private networks only.
- **Compressed builds:** this server does not send `Content-Encoding` headers, so a Brotli or Gzip build only loads here with **Decompression Fallback** enabled in Player Settings → Publishing Settings. Development builds are uncompressed and work as they are.
- **Do not ship** the `*_BurstDebugInformation_DoNotShip` folder that Unity puts next to the build.

## Credits

- **Music:** [Cute and Silly RPG Music Pack](https://chajamakesmusic.itch.io/cute-and-silly-rpg-music-pack) by Chaja Makes Music.
- **Sound effects:** [400 Sounds Pack](https://ci.itch.io/400-sounds-pack) by Chequered Ink.
