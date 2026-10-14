# CV Warehouse

A browser game made in Unity. Robots search a warehouse, take fragments of a CV out of boxes and carry them to a truck, while the document assembles itself live on the waybill.

This is a living document. It is edited as development progresses.

## Game Flow

- **One puzzle:** the whole CV is a single puzzle. Sections only shape the layout of the document, they do not gate the game.
- **Waybill:** there is one waybill and it is the CV. At the start it shows the section titles and the footer, and every block is a greyed-out placeholder already sitting in its final position. That includes the name and the job title.
- **Growing pieces:** a bot carries one word per trip at first and up to five after upgrades, and it also gets faster. The opening is slow enough to watch the name appear word by word, and the rest of the CV speeds up as the player upgrades.
- **Scattered words:** words arrive in no particular order, so a block fills in with gaps before it becomes readable.
- **Roughly top to bottom:** the first blocks of the CV lie closest to the truck, so the document still tends to fill in from the top.
- **Ending:** when the last box is empty and delivered the waybill is the full CV, generated from the same file as the game, with links and a download option. The truck drives away.
- **No fail state:** the result is the time. Left alone, the starting crew keeps delivering, only very slowly.
- **Emergency exit:** a "skip and show CV" button is visible from the first second. It is the way out for a recruiter in a hurry.

## Warehouse

- **Map:** one per game, procedurally generated from the CV file and a seed. The boxes come from the CV, one per block, and the map is sized to them: the generator adds up the floor the boxes take and leaves a set share of the storage floor free, so a longer CV gives a bigger warehouse of the same density. Cluster sizes, ramp size and the shape of the map are parameters. The random seed is visible on screen.
- **A mess, not shelves:** the warehouse is one big open floor with boxes dumped on it. Large boxes land at random, medium and small boxes pile up around them in big clusters, small boxes also gather around medium ones in smaller clusters, and some small boxes stand alone.
- **Buried boxes:** a box in the middle of a pile may have no free side at the start. It opens up when the boxes around it are emptied.
- **Generator guarantees:** every block of the CV gets its box, no box overlaps another or the ramp, and every box can be reached once the boxes in front of it are gone. If the boxes do not fit, the map grows and is generated again.
- **Look:** pixel-art UI on a dark background: Pixelify Sans for text, Silkscreen for small caps labels, bevelled buttons, and the CV on a cream sheet of paper.
- **Warehouse view:** a three-quarter view. The floor is seen from above and everything standing on it is drawn from the front, at 16 pixels per cell. Whatever stands lower on the screen is drawn in front of what stands higher. The floor is a two-tone checkerboard with no marks on it, drop and parking cells included, and the wall beside the truck is brick.
- **Bot art:** one robot in two layers. The body is tinted with the colour of its crew member; the eyes and the antenna tip keep their own colours. A moving bot alternates two step frames, every bot blinks now and then, and a bot hauling text carries one, two or three sheets of paper on its head.
- **Box art:** the size sets the footprint and the importance sets the material, so there are nine boxes. A normal box is plain cardboard with tape, an important one is a wooden crate with planks and steel bands, and a key one is a steel case with gold trim and a gold lock. Every box is drawn open, with paper showing, from the first words taken out of it. An emptied box leaves its flattened remains on the floor in its own material, and bots walk over them.
- **Selection frame:** whatever is selected, a bot or a box, gets steady yellow corner brackets around it.
- **Scanlines:** the warehouse view, and only it, is covered with faint CRT scanlines: one dark line every three screen pixels, whatever the zoom. They are a flat overlay, not a post-processing effect.
- **Vignette:** the corners of the warehouse view fade towards black while the middle stays clear. It is a second flat overlay under the view frame, also without post-processing.
- **Top bar:** runs across the whole screen with the title, the points and the state of the shift (on shift, paused or shipped). It stays visible when the waybill is expanded.
- **Dock:** a strip under the warehouse view holds the counters with the buy and pause buttons, the crew list, and the window of whatever is selected.
- **CV.DOC bar:** the header of the waybill shows a progress bar and the percentage of the letters of the CV delivered so far.
- **Screen:** on a wide screen the warehouse takes the left side and the waybill stays visible the whole time as a panel on the right, so every delivery shows up on the CV straight away. The player scrolls the waybill with the mouse wheel or by dragging it. A button on the waybill expands it to the full screen and back, and the warehouse keeps working while it is expanded. The page narrows to fit the panel, and contacts and links wrap instead of overlapping.
- **Fresh words:** the letters of the latest delivery to a block are marked in yellow on the waybill, and the mark fades away over two seconds. Each block keeps its own mark, so several bots unloading at once do not wipe each other's.
- **Contact icons:** every contact has a small pixel-art icon in front of it, picked by the `kind` of the contact in the CV file: phone, email, github, itch, linkedin or location. Any other kind, or none, gets a generic link icon. The icon takes the colour of the text and stays hidden until the first letter of its contact arrives.
- **Following the deliveries:** when the player has left the waybill alone for five seconds, it scrolls by itself to the block of the latest delivery. It moves only if that block is out of view, glides instead of jumping, and then stays put for at least two seconds. Scrolling the waybill, dragging it, or just keeping the pointer over it counts as using it and stops the following. It never follows while the waybill is expanded to the full screen.
- **AUTO button:** sits in the CV.DOC bar next to Expand and switches the following on and off; it starts on. It is green while on, bright when the waybill is following and dimmer while the player is using the waybill, and grey when off. Off stays off until the player clicks it again. While the waybill is expanded the button cannot be clicked.
- **Camera:** the map is much bigger than the screen. The player uses the keys, rests the mouse pointer at an edge of the warehouse view, or drags with the right or middle mouse button or with a finger to move it, and scrolls to zoom. Dragging with the left mouse button is the selection box. The view starts at the truck.
- **One box per block:** every block of the CV is one box: the name, the job title, each contact, each paragraph and each list entry. The number of boxes does not depend on how the bots carry them.
- **Emptying a box:** a box holds the letters of its block, and each of them leaves the box exactly once. A bot takes out as many whole words as it can carry, or everything that is left if that is less. Which words come out is picked by the seed, and the same words appear on the waybill when the bot unloads. Taking text out of a more important box takes longer, per trip and whatever the bot carries. The box stays in its place until it is empty; then only its flattened remains are left on the floor.
- **Placement:** the boxes that open the CV (name, job title, contacts) lie closest to the truck. The rest are spread over the warehouse by the seed.
- **Boxes:** size shows how much text is inside, the material shows the importance and colour shows the category. Importance uses material, not colour, so the two never compete on one sprite. The text inside is unknown until it arrives on the waybill. The board shows only icons; text appears only on the waybill.
- **Box window:** a click or tap on a box opens a window with its size and word count (for example "Size: large (32 words)"), its importance and its category, and gives the box a highlight. A box without a category says so. The text inside stays unknown. Selecting a bot, clicking empty floor or the close button closes the window, and so does emptying the box. A bot standing on the clicked spot is selected first.
- **Size:** three classes (small, medium, large), set by the number of words in the block. Up to 8 words is small, up to 20 is medium and anything longer is large; both limits are tunable. A bigger box only means more text, so it takes more trips to empty.
- **Importance:** three levels (normal, important, key). Importance sets how much each letter is worth and how long a bot needs to take text out, per trip and whatever the bot carries. It does not limit which bot may carry it. Entries take their importance from the file and are normal when it is missing. The name is key, the job title important and each contact normal, and these three are tunable.
- **Truck:** scenery. It stands at the ramp as the place bots deliver to and drives away when the CV is complete. It has no capacity and no departures during the game.

## Grid and Movement

- **Grid:** the warehouse is a grid of cells. A cell is floor, blocked, a drop cell or a parking cell.
- **Eight directions:** bots step to any of the eight neighbouring cells. A diagonal step costs 1.4 of a straight one and is not allowed past a blocked corner.
- **Linear movement:** a bot slides from cell to cell. Bots on the move drive through each other, so two of them can cross in the same cell. They never pass through boxes.
- **Box footprint:** small 1x1, medium 2x1, large 2x2. A box never moves. When it is emptied its cells become floor that bots can cross, so the map only ever opens up.
- **Access cells:** the floor cells touching a box on its four sides. A bot must stand on one to take text out, and several bots can work one box at once, one per access cell.
- **Truck:** stands on the top edge of the grid. Bots unload on the drop cells in front of it; the rest of the truck is scenery.
- **Parking:** bots without work wait on parking cells by the ramp. The number of parking cells is the limit on bot slots.
- **Tight spots:** gaps between boxes can be one cell wide. Bots that meet in one drive through each other, so a narrow gap never jams.
- **Jobs:** the shared queue hands out the first box, in the order of the bot's focus, that still has unclaimed text and a free access cell. The order is worked out again every time a box disappears.
- **Active pause:** the Space key or the pause button stops the simulation and the same key or button resumes it. While paused the player can still move the camera, select bots, buy bots and upgrades, change focus and read the waybill. A "paused" badge is shown, and no time is caught up after resuming.
- **Removing a bot:** whatever it carries counts as delivered. A bot reserves its access cell, and later its drop cell, so two bots never head for the same spot.
- **Standing bots:** a bot only stops on a cell it has reserved: its access cell, its drop cell or its parking cell. Two bots never stand on the same cell.
- **Fixed tick:** the simulation runs at 60 ticks per second regardless of frame rate, so the same seed always plays out the same way.

## Bots

- **One kind of bot:** every bot is a transport bot. It walks to a box, takes text out and carries it to the truck. Bots share a common task queue.
- **Carry:** how many words a bot takes per trip, from 1 to 5. Every bot starts with one word, including a bot bought later.
- **Speed:** how fast a bot moves between cells, from level 1 to 5. Each level adds a quarter of the starting speed, so a bot at level 5 moves twice as fast. Speed does not change how long taking text out or unloading takes. Every bot starts at level 1.
- **Upgrades belong to a single bot:** the player raises carry or speed on one bot at a time, one level per purchase. The two are bought separately and in any order.
- **Selecting a bot:** a click or tap on a bot selects it, and a click on empty floor deselects it. Every selected bot gets a highlight. The route of every selected bot to where it is heading is drawn as a half-transparent line with rounded corners. Each line takes the crew colour of its bot, and the lines are thinner when several bots are selected.
- **Selection box:** dragging with the left mouse button draws a rectangle, as in a strategy game, and releasing it selects every bot whose centre lies inside. A box that catches no bot clears the selection. The box is anchored to the warehouse, so it grows when the view moves during the drag. Touch has no selection box: a finger drag moves the view.
- **Several bots selected:** a separate window says "Multiple bots selected" and has the same rows as the window of one bot.
  - **Levels:** words per haul and speed of the group, as one number when every bot is at the same level and as a range such as 1-3 when they are not.
  - **Upgrades:** each upgrade button lifts every selected bot one level. Bots already at the last level are left out, and the button is hidden when all of them are. The button says how many bots it covers and the price, for example "Words x3 (120)", which is the sum of what each of those bots would cost alone, with no discount. It is all or nothing: without the points for all of them the button is disabled and nobody is upgraded.
  - **Focus:** shows the focus the bots share, or "Different" when they do not. The arrows give every selected bot the same focus. From "Different" the right arrow starts at the first focus of the cycle and the left arrow at the last; after that they step as for one bot.
  - **Delete and close:** delete removes the selected bots, and the last bot is always kept, also when every bot is selected. Close clears the selection.
- **Bot window:** while a bot is selected a window shows its name, its carry and its speed, each with a button for the next level, its focus, and a button to delete the bot. A button disappears when that upgrade is at its last level. Next to the name the window says what the bot is doing right now, for example "Walking to a box", "Picking up", "Hauling 2 words" or "Loading truck".
- **Names:** every bot has a name and a colour from the crew roster (PIP, BLIP, NIB, MOX, ZEE, TUK, FIG, LUM). A new bot takes the first name nobody in the crew is using, so the name of a removed bot goes to the next bot bought. Bots beyond the roster are numbered ("BOT 9").
- **Crew list:** a panel under the warehouse view lists the whole crew, one chip per bot with its colour and name. A click on a chip selects that bot, and a click on the chip of the selected bot deselects it.
- **Focus:** each bot has a focus that decides which box it takes next, and the player switches it in the bot window for free. *Closest box* takes the box nearest the truck and is the default. *Most important* takes key boxes first, then important, then normal, and the nearest to the truck within one level. *Biggest box* takes large boxes first, then medium, then small, and the nearest to the truck within one size. *Smallest box* is the reverse: small boxes first, then medium, then large, again the nearest to the truck within one size. The cycle runs in that order. The bot window shows one focus at a time, with an arrow on each side to step to the previous or the next one. A bot on its way to a box drops that job and turns to the box of its new focus straight away, also while the game is paused. A bot already taking text out or carrying it back finishes the trip first.
- **Specializations:** a bot can specialize in a category and carries boxes of that category faster.
- **Category:** a list from the file (e.g. technical and soft, up to 4–5). Each gets a colour.
- **Crowding:** more bots does not mean proportionally faster, because a box only has as many access cells as it has open sides and the truck only has a few drop cells. On top of that there is a limit on bot slots.

## Player and Economy

- **Start:** the player begins with a few bots that carry one word each. Nothing else is given for free.
- **Actions:** the player buys and removes bots and buys upgrades and specializations. A removed bot is not refunded, and its upgrades are lost with it. The last bot cannot be removed, so the game can always be finished.
- **Points on screen:** the points the player can spend are shown large in the top bar, above the middle of the warehouse view. The button for buying a bot and, in the bot window, the buttons for that bot's next carry and speed levels show their price and are greyed out until the player can afford them.
- **Currency:** research points, earned for delivered text. Every letter is worth more in a more important box; a bigger box is worth more only because it holds more letters.
- **Shop:** available at all times.
- **Upgrade price:** paid per bot and per level. Every level costs more than the one before, and every bot pays the same price for the same level. Carry and speed are priced separately.
- **Prices:** scaled to the size of the CV, so that a long file does not produce a game without challenge. Every price is a share of all the points in the warehouse.
- **Bot price:** the starting bots are free. Each bought bot costs more than the one before.
- **Upgrades:** named after projects. Effects come from a ready-made pool in code, and an entry in the file points to one of them.

## CV Data

- **JSON file:** name, job title, contacts, list of categories, sections with entries and a footer. The sample lives in `Assets/StreamingAssets/cv_en.json`.
- **Blocks:** the name, the job title, each contact (phone, e-mail, GitHub, itch.io), each `paragraph` section and each entry of a `list` section. One block is one box.
- **Letters and words:** they are not written in the file. The game cuts a block into words and letters by itself.
- **List styles:** `style` only changes the look of a list: `rows`, `bullets`, `keyValue` or `inline`.
- **Entry:** every field is optional and an empty one is not drawn: title, subtitle, meta (date or link text on the right), description, bullets, link, plus the game fields importance (`normal`, `important` or `key`), category and effect. Text may use the `<i>` and `<b>` tags.
- **Reveal:** every block has an id built from its position in the file. The waybill shows a grey placeholder for it, and each delivered letter or word appears in its own place inside the block.
- **Footer and section titles:** shown from the start, they are not boxes.
- **Extensibility:** adding or removing entries changes the number of boxes and the map size without touching code.
- **Edge cases:** an empty section is skipped.
- **Robustness:** missing fields get default values, an invalid file shows a readable message.
- **Swapping:** the file can be replaced on the server without rebuilding the game, and a second language version is a second file.
