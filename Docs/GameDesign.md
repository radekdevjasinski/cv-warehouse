# CV Warehouse

A browser game made in Unity. Robots search a warehouse, take fragments of a CV out of boxes and carry them to a truck, while the document assembles itself live on the waybill.

This is a living document. It is edited as development progresses.

## Game Flow

- **One puzzle:** the whole CV is a single puzzle. Sections only shape the layout of the document, they do not gate the game.
- **Waybill:** there is one waybill and it is the CV. At the start it shows the section titles and the footer, and every block is a greyed-out placeholder already sitting in its final position. That includes the name and the job title.
- **Growing pieces:** bots first carry one letter at a time, then one word, then a whole block. The name is spelled out letter by letter, and the rest of the CV speeds up as the player upgrades.
- **Scattered letters:** letters and words arrive in no particular order, so a block fills in with gaps before it becomes readable.
- **Roughly top to bottom:** the first blocks of the CV lie closest to the truck, so the document still tends to fill in from the top.
- **Ending:** when the last box is empty and delivered the waybill is the full CV, generated from the same file as the game, with links and a download option. The truck drives away.
- **No fail state:** the result is the time. Left alone, the starting crew keeps delivering, only very slowly.
- **Emergency exit:** a "skip and show CV" button is visible from the first second. It is the way out for a recruiter in a hurry.

## Warehouse

- **Map:** one per game, procedurally generated from parameters (map size, box counts, cluster sizes, ramp size). The random seed is visible on screen.
- **A mess, not shelves:** the warehouse is one big open floor with boxes dumped on it. Heavy boxes land at random, medium and light boxes pile up around them in big clusters, light boxes also gather around medium ones in smaller clusters, and some light boxes stand alone.
- **Buried boxes:** a box in the middle of a pile may have no free side at the start. It opens up when the boxes around it are emptied.
- **Generator guarantees:** every box is placed, no box overlaps another or the ramp, and every box can be reached once the boxes in front of it are gone.
- **Camera:** the map is much bigger than the screen. The player drags or uses the keys to move the view and scrolls to zoom. The view starts at the truck.
- **One box per block:** every block of the CV is one box: the name, the job title, each contact, each paragraph and each list entry. The number of boxes does not depend on how the bots carry them.
- **Emptying a box:** a bot takes out as much as it can carry: one of the remaining letters, the rest of one word, or everything that is left. Which letter or word comes out is picked by the seed. The box stays on its shelf until it is empty and then disappears. A bot that carries blocks empties a box in one trip.
- **Placement:** the boxes that open the CV (name, job title, contacts) lie closest to the truck. The rest are spread over the warehouse by the seed.
- **Boxes:** size shows the weight class and colour shows the category. The text inside is unknown until it arrives on the waybill. The board shows only icons; text appears only on the waybill.
- **Weight:** three classes (light, medium, heavy). Weight is the importance of a block and sets how much it is worth. It does not limit which bot may carry it.
- **Truck:** scenery. It stands at the ramp as the place bots deliver to and drives away when the CV is complete. It has no capacity and no departures during the game.

## Grid and Movement

- **Grid:** the warehouse is a grid of cells. A cell is floor, blocked, a drop cell or a parking cell.
- **Eight directions:** bots step to any of the eight neighbouring cells. A diagonal step costs 1.4 of a straight one and is not allowed past a blocked corner or past another bot.
- **Linear movement:** a bot slides from cell to cell and holds both cells until it arrives. Two bots never share a cell and never pass through each other or through boxes.
- **Box footprint:** light 1x1, medium 2x1, heavy 2x2. A box never moves. When it is emptied it disappears and its cells become floor, so the map only ever opens up.
- **Access cells:** the floor cells touching a box on its four sides. A bot must stand on one to take text out, and several bots can work one box at once, one per access cell.
- **Truck:** stands on the top edge of the grid. Bots unload on the drop cells in front of it; the rest of the truck is scenery.
- **Parking:** bots without work wait on parking cells by the ramp. The number of parking cells is the limit on bot slots.
- **Tight spots:** gaps between boxes can be one cell wide. Bots that meet in one wait, look for a way around and step aside, so a jam slows them down without locking them.
- **Jobs:** the shared queue hands out the box nearest the truck that still has unclaimed text and a free access cell. The order is worked out again every time a box disappears.
- **Removing a bot:** whatever it carries counts as delivered. A bot reserves its access cell, and later its drop cell, so two bots never head for the same spot.
- **Blocked bots:** a bot whose next cell is taken waits, then looks for a path around the other bots, then steps aside to a free cell picked by the seed.
- **Fixed tick:** the simulation runs at 60 ticks per second regardless of frame rate, so the same seed always plays out the same way.

## Bots

- **One kind of bot:** every bot is a transport bot. It walks to a box, takes text out and carries it to the truck. Bots share a common task queue.
- **Carry size:** how much text a bot takes per trip: a letter, a word or a block. Bots start with a letter. Carry size is raised by upgrades.
- **Specializations:** a bot can specialize in a category and carries boxes of that category faster.
- **Category:** a list from the file (e.g. technical and soft, up to 4–5). Each gets a colour.
- **Traffic jams:** more bots does not mean faster, because aisles are narrow. On top of that there is a limit on bot slots.

## Player and Economy

- **Start:** the player begins with a few bots that carry letters. Nothing else is given for free.
- **Actions:** the player places and removes bots and buys upgrades and specializations.
- **Currency:** research points, earned for delivered text. Heavier boxes are worth more.
- **Shop:** available at all times.
- **First upgrade:** carrying a word is affordable right after the name has been delivered, whatever the size of the CV. The letter stage is a short opening, not a grind.
- **Prices:** scaled to the size of the CV, so that a long file does not produce a game without challenge.
- **Upgrades:** named after projects. Effects come from a ready-made pool in code, and an entry in the file points to one of them.

## CV Data

- **JSON file:** name, job title, contacts, list of categories, sections with entries and a footer. The sample lives in `Assets/StreamingAssets/cv_en.json`.
- **Blocks:** the name, the job title, each contact (phone, e-mail, GitHub, itch.io), each `paragraph` section and each entry of a `list` section. One block is one box.
- **Letters and words:** they are not written in the file. The game cuts a block into words and letters by itself.
- **List styles:** `style` only changes the look of a list: `rows`, `bullets`, `keyValue` or `inline`.
- **Entry:** every field is optional and an empty one is not drawn: title, subtitle, meta (date or link text on the right), description, bullets, link, plus the game fields weight, category and effect. Text may use the `<i>` and `<b>` tags.
- **Reveal:** every block has an id built from its position in the file. The waybill shows a grey placeholder for it, and each delivered letter or word appears in its own place inside the block.
- **Footer and section titles:** shown from the start, they are not boxes.
- **Extensibility:** adding or removing entries changes the number of boxes and the map size without touching code.
- **Edge cases:** an empty section is skipped.
- **Robustness:** missing fields get default values, an invalid file shows a readable message.
- **Swapping:** the file can be replaced on the server without rebuilding the game, and a second language version is a second file.
