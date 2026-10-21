# CV Warehouse

A browser game made in Unity. Robots search a warehouse, take fragments of a CV out of boxes and carry them to a truck, while the document assembles itself live on the waybill.

This is a living document. It is edited as development progresses.

## Game Flow

- **One puzzle:** the whole CV is a single puzzle. Sections only shape the layout of the document, they do not gate the game.
- **Waybill:** there is one waybill and it is the CV. At the start it shows the section titles and the footer, and every word of every block is a grey bar already sitting in its final position, as wide as the word it hides. That includes the name and the job title.
- **Growing pieces:** a bot carries one word per trip at first and up to five after upgrades, and it also gets faster. The opening is slow enough to watch the name appear word by word, and the rest of the CV speeds up as the player upgrades.
- **Scattered words:** words arrive in no particular order, so a block fills in with gaps before it becomes readable.
- **Roughly top to bottom:** the first blocks of the CV lie closest to the truck, so the document still tends to fill in from the top.
- **Ending:** when the last box is empty and delivered the waybill is the full CV, generated from the same file as the game, with links and a download option. The truck drives away.
- **No fail state:** the result is the time. Left alone, the starting crew keeps delivering, only very slowly.
- **Emergency exit:** a "skip and show CV" button is visible from the first second. It is the way out for a recruiter in a hurry.

## Menus

- **Main menu:** the game opens on a main menu. It is the first scene of the build and stays small, so the page becomes interactive quickly. At the top is the gold Silkscreen title "cv-warehouse" with two hard shadows, one dark gold and one near-black. Under it the subtitle "an interactive CV by" is followed by the name from the CV file, or ends after "CV" if the file cannot be read. In the middle is a dark panel with a yellow-and-black hazard stripe along its top and bevelled buttons: START SHIFT in green, then View CV (marked "skip the game"), How to play and Settings, and a sound button below them. GitHub and LinkedIn links and the build version sit in the bottom right corner. The game is English only, so there is no language switch, and there is no Exit button because a browser page cannot close itself. For now only START SHIFT works.
- **Menu backdrop:** a close-up of the bare warehouse floor, without walls, in the same 16-pixel art and whole-step pixel-perfect zoom as the game. A few boxes stand around, and one bot walks back and forth between two open boxes: it takes paper out of one, carries it over on its head and puts it into the other, forever. The timings live in a settings asset.
- **Game menu:** a Menu button in the top bar, or Esc, opens a panel with Resume, Restart, Show CV and Main menu. While it is open the warehouse stands still and the shift shows as paused; closing it does not undo a pause the player set before. Restart starts the run again with the same CV and seed. Show CV reveals the whole CV and expands the waybill, and the warehouse keeps working behind it. Main menu leaves the run.

## Warehouse

- **Map:** one per game, procedurally generated from the CV file and a seed. The boxes come from the CV, one per block, and the map is sized to them: the generator adds up the floor the boxes take and leaves a set share of the storage floor free, so a longer CV gives a bigger warehouse of the same density. The density is high on purpose: boxes take about a fifth of the storage floor, so the warehouse feels cramped and the bots stay in view. Cluster sizes, ramp size and the shape of the map are parameters. The random seed is visible on screen.
- **A mess, not shelves:** the warehouse is one big open floor with boxes dumped on it. Large boxes land at random, medium and small boxes pile up around them in big clusters, small boxes also gather around medium ones in smaller clusters, and some small boxes stand alone.
- **Buried boxes:** a box in the middle of a pile may have no free side at the start. It opens up when the boxes around it are emptied.
- **Generator guarantees:** every block of the CV gets its box, no box overlaps another or the ramp, and every box can be reached once the boxes in front of it are gone. If the boxes do not fit, the map grows and is generated again.
- **Look:** pixel-art UI on a dark background: Pixelify Sans for text, Silkscreen for small caps labels, bevelled buttons, and the CV on a cream sheet of paper.
- **Page:** the game fills the whole browser window. A window narrower than 4:3 does not squeeze the game further: it keeps the full width, its height stops at 4:3, and plain bars in the background colour fill the space above and below.
- **Warehouse view:** a three-quarter view. The floor is seen from above and everything standing on it is drawn from the front, at 16 pixels per cell. Whatever stands lower on the screen is drawn in front of what stands higher. The floor is a two-tone checkerboard with no marks on it, drop and parking cells included, and the wall beside the truck is brick.
- **Bot art:** one robot in two layers. The body is tinted with the colour of its crew member; the eyes and the antenna tip keep their own colours. A moving bot alternates two step frames, every bot blinks now and then, and a bot hauling text carries one, two or three sheets of paper on its head.
- **Box art:** the size sets the footprint and the importance sets the material, so there are nine boxes. A normal box is plain cardboard with tape, an important one is a wooden crate with planks and steel bands, and a key one is a steel case with gold trim and a gold lock. Every box is drawn open, with paper showing, from the first words taken out of it. An emptied box leaves its flattened remains on the floor in its own material, and bots walk over them.
- **Truck art:** the wall along the top of the map is four cells tall, and the truck is parked against it seen from the side, facing left: a short cardboard-orange cab and a long dark navy box with faint stripes and a big gold "CV" logo in the middle, on a dark grey chassis. It uses the same palette, the same 1-pixel outline and the same pixel size as the bots and boxes. It has a wheel under the cab, a drive wheel just behind it and a group of wheels at the back, and a longer truck gets more wheels in that back group. Behind it one big open garage door, wider and taller than the truck, shows a rolled-up shutter, a dark opening and a yellow-dashed sill; brick fills the rest of the wall. The truck and the door are built to fit whatever width the generator gives the truck.
- **Selection frame:** whatever is selected, a bot or a box, gets steady yellow corner brackets around it.
- **Scanlines:** the warehouse view, and only it, is covered with faint CRT scanlines: one dark line every three screen pixels, whatever the zoom. They are a flat overlay, not a post-processing effect.
- **Vignette:** the corners of the warehouse view fade towards black while the middle stays clear. It is a second flat overlay under the view frame, also without post-processing.
- **Top bar:** runs across the whole screen with the title, the points, the state of the shift (on shift, paused or shipped) and the Menu button. It stays visible when the waybill is expanded.
- **Dock:** a strip under the warehouse view holds the bot counter with the buy and pause buttons, the crew list, and the window of whatever is selected.
- **CV.DOC bar:** the header of the waybill shows a progress bar and the percentage of the letters of the CV delivered so far.
- **Screen:** on a wide screen the warehouse takes the left side and the waybill stays visible the whole time as a panel on the right, so every delivery shows up on the CV straight away. The player scrolls the waybill with the mouse wheel or by dragging it. A button on the waybill expands it to the full screen and back, and the warehouse keeps working while it is expanded. The page narrows to fit the panel, and contacts and links wrap instead of overlapping.
- **Fresh words:** the letters of the latest delivery to a block are marked in yellow on the waybill, and the mark fades away over two seconds. Each block keeps its own mark, so several bots unloading at once do not wipe each other's.
- **Contact icons:** every contact has a small pixel-art icon in front of it, picked by the `kind` of the contact in the CV file: phone, email, github, itch, linkedin or location. Any other kind, or none, gets a generic link icon. The icon takes the colour of the text and stays hidden until the first letter of its contact arrives.
- **Following the deliveries:** when the player has left the waybill alone for five seconds, it scrolls by itself to the block of the latest delivery. It moves only if that block is out of view, glides instead of jumping, and then stays put for at least two seconds. Scrolling the waybill, dragging it, or just keeping the pointer over it counts as using it and stops the following. It never follows while the waybill is expanded to the full screen.
- **AUTO button:** sits in the CV.DOC bar next to Expand and switches the following on and off; it starts on. It is green while on, bright when the waybill is following and dimmer while the player is using the waybill, and grey when off. Off stays off until the player clicks it again. While the waybill is expanded the button cannot be clicked.
- **Camera:** the map is bigger than the screen. The player uses the keys or drags with the right or middle mouse button or with a finger to move it, and scrolls to zoom. Dragging with the left mouse button is the selection box. The view starts at the truck, zoomed out as far as it goes.
- **Pixel-perfect zoom:** zoom moves in whole steps, so every pixel of the 16-pixel art is always drawn as a square of whole screen pixels (2x2, 3x3 and so on), and the view is snapped to the screen pixel grid. The zoom range is set in cells on the short side of the view, so it looks the same on any screen: the closest zoom shows about 6 cells, the farthest at most 24. The view never leaves the warehouse: the camera stops at its walls, and the farthest zoom is also limited so that the view fits inside the map.
- **One box per block:** every block of the CV is one box: the name, the job title, each contact, each paragraph and each list entry. The number of boxes does not depend on how the bots carry them.
- **Emptying a box:** a box holds the letters of its block, and each of them leaves the box exactly once. A bot takes out as many whole words as it can carry, or everything that is left if that is less. Which words come out is picked by the seed, and the same words appear on the waybill when the bot unloads. Taking text out of a more important box takes longer, per trip and whatever the bot carries. The box stays in its place until it is empty; then only its flattened remains are left on the floor.
- **Placement:** the boxes that open the CV (name, job title, contacts) lie closest to the truck. The rest are spread over the warehouse by the seed.
- **Boxes:** size shows how much text is inside, the material shows the importance and colour shows the category. Importance uses material, not colour, so the two never compete on one sprite. The text inside is unknown until it arrives on the waybill. The board shows only icons; text appears only on the waybill.
- **Box window:** a click or tap on a box opens a window with its size, its importance and its category, and gives the box a highlight. A box without a category says so. The text inside stays unknown. Selecting a bot, clicking empty floor or the close button closes the window, and so does emptying the box. A bot standing on the clicked spot is selected first.
- **Size:** three classes (small, medium, large), set by the number of words in the block. Up to 8 words is small, up to 20 is medium and anything longer is large; both limits are tunable. A bigger box only means more text, so it takes more trips to empty.
- **Importance:** three levels (normal, important, key). Importance sets how much each letter is worth and how long a bot needs to take text out, per trip and whatever the bot carries: half a second for a normal box, one second for an important one and two seconds for a key one. While a bot takes text out, a small pixel-art progress bar under it fills up in whole pixels and disappears when the bot leaves with its load. It does not limit which bot may carry it. Entries take their importance from the file and are normal when it is missing. The name is key, the job title important and each contact normal, and these three are tunable.
- **Truck:** scenery. It stands at the ramp as the place bots deliver to and drives away when the CV is complete. It has no capacity and no departures during the game.

## Grid and Movement

- **Grid:** the warehouse is a grid of cells. A cell is floor, blocked, a drop cell or a parking cell.
- **Eight directions:** bots step to any of the eight neighbouring cells. A diagonal step costs 1.4 of a straight one and is not allowed past a blocked corner.
- **Linear movement:** a bot slides from cell to cell. Bots on the move drive through each other, so two of them can cross in the same cell. They never pass through boxes.
- **Box footprint:** small 1x1, medium 2x1, large 2x2. A box never moves. When it is emptied its cells become floor that bots can cross, so the map only ever opens up.
- **Access cells:** the floor cells touching a box on its four sides. A bot must stand on one to take text out, and several bots can work one box at once, one per access cell.
- **Truck:** stands in the four-row wall on the top edge of the grid. Bots unload on the drop cells in front of it; the rest of the truck is scenery.
- **Parking:** bots without work wait on parking cells by the ramp, four on each side of the drop cells, so there are at most eight bots. The number of parking cells is the limit on bot slots. The ramp is three rows deep.
- **Tight spots:** gaps between boxes can be one cell wide. Bots that meet in one drive through each other, so a narrow gap never jams.
- **Jobs:** the shared queue hands out the first box, in the order of the bot's focus, that still has unclaimed text and a free access cell. The order is worked out again every time a box disappears.
- **Active pause:** the Space key or the pause button stops the simulation and the same key or button resumes it. While paused the player can still move the camera, select bots, buy bots and upgrades, change focus and read the waybill. A "paused" badge is shown, and no time is caught up after resuming.
- **Removing a bot:** whatever it carries counts as delivered. A bot reserves its access cell, and later its drop cell, so two bots never head for the same spot.
- **Standing bots:** a bot only stops on a cell it has reserved: its access cell, its drop cell or its parking cell. Two bots never stand on the same cell.
- **Fixed tick:** the simulation runs at 60 ticks per second regardless of frame rate, so the same seed always plays out the same way.

## Bots

- **One kind of bot:** every bot is a transport bot. It walks to a box, takes text out and carries it to the truck. Bots share a common task queue.
- **Carry:** how many words a bot takes per trip, from 1 to 5. Every bot starts with one word, including a bot bought later.
- **Speed:** how fast a bot moves between cells, from level 1 to 5. A bot at level 1 needs a quarter of a second to cross one cell. Each level adds a quarter of the starting speed, so a bot at level 5 moves twice as fast, an eighth of a second per cell. Speed does not change how long taking text out or unloading takes. Every bot starts at level 1.
- **Upgrades belong to a single bot:** the player raises carry or speed on one bot at a time, one level per purchase. The two are bought separately and in any order.
- **Selecting a bot:** a click or tap on a bot selects it, and a click on empty floor deselects it. Every selected bot gets a highlight. The route of every selected bot to where it is heading is drawn as a half-transparent line with rounded corners. Each line takes the crew colour of its bot, and the lines are thinner when several bots are selected.
- **Selection box:** dragging with the left mouse button draws a rectangle, as in a strategy game, and releasing it selects every bot whose centre lies inside. A box that catches no bot clears the selection. The box is anchored to the warehouse, so it grows when the view moves during the drag. Touch has no selection box: a finger drag moves the view.
- **Several bots selected:** a separate window says "Multiple bots selected" and has the same rows as the window of one bot.
  - **Levels:** words per haul and speed of the group, as one number when every bot is at the same level and as a range such as 1-3 when they are not.
  - **Upgrades:** each upgrade button lifts every selected bot one level. Bots already at the last level are left out, and when all of them are, the button stays in place, says "Max" and cannot be clicked. The button says how many bots it covers and the price, for example "Words x3 (120)", which is the sum of what each of those bots would cost alone, with no discount. It is all or nothing: without the points for all of them the button is disabled and nobody is upgraded.
  - **Focus:** shows the focus the bots share, or "Different" when they do not. The arrows give every selected bot the same focus. From "Different" the right arrow starts at the first focus of the cycle and the left arrow at the last; after that they step as for one bot.
  - **Delete and close:** delete removes the selected bots, and the last bot is always kept, also when every bot is selected. Close clears the selection.
- **Bot window:** while a bot is selected a window shows its name, its carry and its speed, each with a button for the next level, its focus, and a button to delete the bot. When that upgrade is at its last level the button stays in place, turns grey, says "Max" and cannot be clicked. Next to the name the window says what the bot is doing right now, for example "Walking to a box", "Picking up", "Hauling 2 words" or "Loading truck".
- **Names:** every bot has a name and a colour from the crew roster (PIP, BLIP, NIB, MOX, ZEE, TUK, FIG, LUM). A new bot takes the first name nobody in the crew is using, so the name of a removed bot goes to the next bot bought. Bots beyond the roster are numbered ("BOT 9").
- **Crew list:** a panel under the warehouse view lists the whole crew, one chip per bot with its colour and name. A click on a chip selects that bot, and a click on the chip of the selected bot deselects it. Holding Shift or Ctrl while clicking adds the bot to the selection, or takes it out if it is already selected, so several bots can be picked from the list. A click on the empty space of the panel, around the chips, clears the selection, just like a click on empty floor.
- **Focus:** each bot has a focus that decides which box it takes next, and the player switches it in the bot window for free. *Closest box* takes the box nearest the truck and is the default. *Most important* takes key boxes first, then important, then normal, and the nearest to the truck within one level. *Biggest box* takes large boxes first, then medium, then small, and the nearest to the truck within one size. *Smallest box* is the reverse: small boxes first, then medium, then large, again the nearest to the truck within one size. The cycle runs in that order. The bot window shows one focus at a time, with an arrow on each side to step to the previous or the next one. A bot on its way to a box drops that job and turns to the box of its new focus straight away, also while the game is paused. A bot already taking text out or carrying it back finishes the trip first.
- **Category:** a list from the file (e.g. technical and soft, up to 4–5). Each gets a colour. A category only decides the colour of a box and is shown in the box window; it has no effect on bots, points or prices.
- **Crowding:** more bots does not mean proportionally faster, because a box only has as many access cells as it has open sides and the truck only has a few drop cells. On top of that there is a limit on bot slots.

## Player and Economy

- **Start:** the player begins with a few bots that carry one word each. Nothing else is given for free.
- **Actions:** the player buys and removes bots and buys upgrades. A removed bot is not refunded, and its upgrades are lost with it. The last bot cannot be removed, so the game can always be finished.
- **Points on screen:** the points the player can spend are shown large in the top bar, above the middle of the warehouse view. The button for buying a bot and, in the bot window, the buttons for that bot's next carry and speed levels show their price. An upgrade button the player can afford turns green; one they cannot afford yet stays blue and faded. The buy bot button follows the same colours: green when the player can afford a bot, blue and faded until then, and grey when every parking slot is taken.
- **Points at the truck:** when a bot finishes unloading, the points it just earned float up above it in the same gold as the points in the top bar with a black outline, for example "+24", and fade out within about a second. Popups from bots unloading side by side stack above each other instead of overlapping. A deleted bot whose letters count as delivered shows nothing, because it never reached the truck.
- **Points counter pulse:** whenever the points in the top bar go up, the number swells briefly and settles back. Spending points does not pulse it.
- **New bot:** a bought bot pops in at its parking cell, growing from nothing with a small overshoot. The bots the game starts with are simply there.
- **Upgrade flash:** every bot that gets an upgrade flashes white for a moment and fades back to its crew colour.
- **Currency:** research points, earned for delivered text. Every letter is worth more in a more important box; a bigger box is worth more only because it holds more letters.
- **Shop:** available at all times.
- **Upgrade price:** paid per bot and per level. Every level costs more than the one before, and every bot pays the same price for the same level. Carry and speed are priced separately.
- **Prices:** scaled to the size of the CV, so that a long file does not produce a game without challenge. Every price is a share of all the points in the warehouse.
- **Bot price:** the starting bots are free. Each bought bot costs more than the one before.

## CV Data

- **JSON file:** name, job title, contacts, list of categories, sections with entries and a footer. The sample lives in `Assets/StreamingAssets/cv_en.json`.
- **Blocks:** the name, the job title, each contact (phone, e-mail, GitHub, itch.io), each `paragraph` section and each entry of a `list` section. One block is one box.
- **Letters and words:** they are not written in the file. The game cuts a block into words and letters by itself.
- **Decorations:** a piece of text between spaces with no letter or digit in it, such as the bullet in front of a list entry, the dash between a title and its subtitle or the dash in "2017 - 2021", is a decoration. It is shown on the waybill from the start, never gets a bar, and is not a letter in any box. Punctuation stuck to a word, such as the comma in "Unity,", stays part of that word.
- **List styles:** `style` only changes the look of a list: `rows`, `bullets`, `keyValue` or `inline`.
- **Entry:** every field is optional and an empty one is not drawn: title, subtitle, meta (date or link text on the right), description, bullets, link, plus the game fields importance (`normal`, `important` or `key`) and category. Text may use the `<i>` and `<b>` tags.
- **Reveal:** every block has an id built from its position in the file. The waybill lays out the full text of the block from the start and covers each hidden word with a grey bar. Each delivered letter or word appears in its own place, and the bar under a word goes away once all its letters are delivered.
- **Footer and section titles:** shown from the start, they are not boxes.
- **Extensibility:** adding or removing entries changes the number of boxes and the map size without touching code.
- **Edge cases:** an empty section is skipped.
- **Robustness:** missing fields get default values, an invalid file shows a readable message.
- **Swapping:** the file can be replaced on the server without rebuilding the game, and a second language version is a second file.

## Audio

- **Music:** the tracks in `Assets/Audio/Music` play one after another in a fixed order, and after the last one the first starts again. Music never stops for the pause button or the end of the game.
- **Mix:** music and sound effects go to their own groups in one Audio Mixer. The effects group is 30% quieter than the music group, so effects never drown the music. Each effect also has its own volume, pitch and pitch variation in a settings asset, which can be tuned while the game runs.
- **Extract sound:** a short sound plays when a bot takes letters out of a box.
- **Delivery sound:** a short sound plays when a bot unloads letters at the truck, so a busy truck sounds like bubbles bursting rather than one repeated click.
- **Bot voices:** every crew slot has its own voice, made with sfxr, in the same order as the crew names and colours, so a bot keeps its voice for the whole game and a ninth bot wraps around to the first voice. Clicking a bot plays its voice with a very small pitch change, and so does buying a new bot.
- **Selecting several bots:** their voices play one after another in crew order, a moment apart, like a strummed chord. Each voice plays once even when two selected bots share it. At most four voices play, picked at random from the selection each time, and each is a little quieter than a single voice. Deselecting a bot plays nothing.
- **Upgrade sounds:** buying an upgrade plays a power-up sound, a different one for more words and for more speed. Upgrading a group of bots plays it once.
- **Variation:** every effect picks a random clip, never the same one twice in a row, at a slightly random pitch. Plays of the same effect closer together than a short cooldown are skipped, so many bots acting in the same moment do not stack into noise.
- **Browser start:** the browser lets sound play only after the first click or tap, so the first track waits for it.
- **Download size:** music is mono AAC at the lowest quality the encoder offers, loaded only when its track starts, and a finished track is unloaded.
- **Credits:** the music comes from the [Cute and Silly RPG Music Pack](https://chajamakesmusic.itch.io/cute-and-silly-rpg-music-pack) by Chaja Makes Music, and the sound effects from the [400 Sounds Pack](https://ci.itch.io/400-sounds-pack) by Chequered Ink, both credited in the README.
