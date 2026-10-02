# CV Warehouse

A browser game made in Unity. Robots search a warehouse, find boxes containing fragments of a CV and load them onto trucks, while the document assembles itself live on the waybill.

This is a living document. It is edited as development progresses.

## Game Flow

- **Trucks:** each CV section is one truck with its own order.
- **Waybill:** the waybill is the CV. At the start it has a header with the name and job title plus greyed-out line items, and every loaded box fills in one line.
- **Departure:** closes the section and gives a short pause with a summary and a time bonus.
- **Ending:** all waybills combine into the full CV, generated from the same file as the game, with links and a download option.
- **No fail state:** the result is the time, and the free starting crew delivers everything on its own, only slower.
- **Emergency exit:** a "skip and show CV" button is visible from the first second.

## Warehouse

- **Map:** one per game, procedurally generated from parameters (shelf rows, aisle width, cross passages, ramp position). The random seed is visible on screen.
- **Generator guarantees:** everything is reachable, there are no one-tile dead-end aisles, and there are enough shelves for all entries.
- **Accessibility:** the whole warehouse is open from the start, and all entries lie in closed boxes.
- **Boxes:** size reveals the weight class, the contents are hidden until scanned. The board shows only icons; text appears only on the waybill.

## Bots

- **Two stages of work:** a scanner inspects a box, and only then does a transport bot pick it up. Bots share a common task queue.
- **Cataloguing:** the scanner remembers which section an inspected box belongs to, so boxes from other sections wait, tagged, for their truck.
- **Types:**
  - **Scanner:** carries nothing, reveals contents.
  - **Porter:** strong and slow, carries everything, blocks narrow aisles.
  - **Courier:** fast, carries only light and medium boxes.
- **Weight:** three classes (light, medium, heavy), a hard limit on who can lift what.
- **Category:** a list from the file (e.g. technical and soft, up to 4–5). Each gets a colour and a specialist bot variant that carries it faster.
- **Traffic jams:** more bots does not mean faster, because aisles are narrow. On top of that there is a limit on bot slots.

## Player and Economy

- **Actions:** the player places and removes bots and buys upgrades.
- **Currency:** research points. A few for an inspected box, more for a loaded one, a time bonus at departure.
- **Shop:** available at all times, with no upgrade choice on section change.
- **Prices:** scaled to the size of the CV, so that a long file does not produce a game without challenge.
- **Upgrades:** named after projects. Effects come from a ready-made pool in code, and an entry in the file points to one of them.

## CV Data

- **JSON file:** name, job title, list of categories, sections with entries. An entry has a title, description, weight, category and optionally an effect and a link.
- **Extensibility:** adding or removing a section changes the number of trucks and the map size without touching code.
- **Edge cases:** an empty section is skipped, a truck gets 2–6 boxes, a longer section is split into two.
- **Robustness:** missing fields get default values, an invalid file shows a readable message.
- **Swapping:** the file can be replaced on the server without rebuilding the game, and a second language version is a second file.
