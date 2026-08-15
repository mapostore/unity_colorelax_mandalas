-------------------------------------------------------------------------
--T and G Apps Ltd.
--www.tandgapps.co.uk
--support@tandgapps.co.uk

--CoronaSDK version 2014.2189a was used for this template.

--You are not allowed to publish this template to the Appstore as it is.
--You need to work on it, improve it and replace the graphics.

--For questions and/or bugs found, please contact me using our contact
--form on http://www.tandgapps.co.uk/contact-us/
-------------------------------------------------------------------------

-- Create our local M table
local M = {}

-- Localise a few other helpful vars
local sqlite3 = require("sqlite3")
local mF = math.floor
local _W = display.contentWidth
local _H = display.contentHeight


-- Import our sharing file
-- Please ntoe that only facebook social sharing is used for iOS currently due to the IDFA issues with Apple.
M.sharing = require("helpers.sharing") 
M.shareMessage = "I'm playing this awesome app, come and join me! http://www.awesomeapp.com/"
M.sharing.facebookId = "PUT_YOUR_FACEBOOK_ID_HERE"  --This needs to be changed if you want to use native facebook sharing.



--------------------------------------------
-- VARIABLES
--------------------------------------------
-- Give them some default values here.
-- These are all put in the M table so that we can access them easily elsewhere.
M.highscore = 0
M.gems = 0
M.powerupInfo = { 1, 1, 1 }
M.sensitivity = "medium"
M.soundOn = true
M.ballCost = 1    --This doubles each time we do it, until we restart the game completely!
M.pixelFont = "PC Senior"
M.platform = "iOS"
if system.getInfo("platformName") == "Android" then
	M.platform = "Android"
	M.pixelFont = "pcsenior"
end



--------------------------------------------
-- ADS - VUNGLE
--------------------------------------------
-- To use video ads you MUST follow the following steps
-- 		1. Sign up to Vungle here: https://v.vungle.com/dashboard/signup
--		2. Create an application and get an "app ID"
--		3. Replace the appId below with the one you just got.
-- 		4. Build the app and test on your device. 
--------------------------------------------
local provider = "vungle"
local appId = "vungleTest"  -- Replace this with your own vungle application ID
if M.platform == "Android" then -- Different platforms can have different appId's, so change it here if needed. 
	appId = "vungleTest" -- Replace this with your own android vungle application ID
end
M.ads = require "ads"
M.canShowAd = true 	--We only allow one video view per game. 


-- Show an ad if one has been downloaded and is available for playback
M.showAd = function ()
	M.ads.show( "interstitial", { isBackButtonEnabled = false, isCloseShown=false } )  --Forces the user to watch the whole video
end

-- Listen for Ad events. 
-- We don't really need to do much here, but you can do if you want. E.g. error checking / caching variables. 
local function functionAdListener( event )
	-- Video ad downloaded and available
	if event.type == "cachedAdAvailable" then
		
	-- Video ad displayed and then closed
	elseif event.type == "adEnd" then
		print("Ad is now going to close as its finished")
		print("Do we need to enable a close button or anything? ")

	else
		print( "------ Received event:")
		print( "-- Name:", event.name )
		print( "-- Provider:", event.provider )
		print( "-- Type:", event.type )
		print( "-- Response:", event.response )
	end
end

-- Initialise the ad listener
M.ads.init( provider, appId, functionAdListener )



--------------------------------------------
-- SPRITESHEET
--------------------------------------------
-- Create the main spritesheet that we will use throughout the app. 
-- This was made in texture packer. When you change the images on it by creating your own, it would be a good idea to keep the file names the same, that means you won't have to change the code in many places.
-- You can show any frame of the sprite sheet by simply doing: local sprite = display.newSprite( spriteSheetImage , {frames={spriteSheetData:getFrameIndex("logo")}} )
--[[ 
    ["brick_greyscale_defense"] = 1,
    ["brick_greyscale_gem"] = 2,
    ["brick_greyscale_light"] = 3,
    ["brick_greyscale_normal"] = 4,
    ["btn_cup"] = 5,
    ["btn_normal_blue"] = 6,
    ["btn_normal_green"] = 7,
    ["btn_normal_pink"] = 8,
    ["btn_small_black"] = 9,
    ["btn_small_blue"] = 10,
    ["btn_small_grey"] = 11,
    ["btn_tall_blue"] = 12,
    ["btn_tall_grey"] = 13,
    ["btn_wide2_grey"] = 14,
    ["btn_wide_green"] = 15,
    ["btn_wide_grey"] = 16,
    ["gem_large"] = 17,
    ["gem_small"] = 18,
    ["hint"] = 19,
    ["logo"] = 20,
    ["paddle_ball"] = 21,
    ["paddle_small_laser"] = 22,
    ["paddle_small_normal"] = 23,
    ["paddle_small_normal_wide"] = 24,
    ["paddle_small_shadow"] = 25,
    ["paddle_small_shadow_wide"] = 26,
    ["powerup_laser"] = 27,
    ["powerup_pill_bomb"] = 28,
    ["powerup_pill_extend"] = 29,
    ["powerup_pill_laser"] = 30,
    ["settings"] = 31,
    ["upgrade_block_lit"] = 32,
    ["upgrade_block_unlit"] = 33,
]]
M.spriteSheetData = require("images.spriteSheet")
M.spriteSheetImage = graphics.newImageSheet( "images/spriteSheet.png", M.spriteSheetData:getSheet() )



--------------------------------------------
-- DATABASES - LOADING/SAVING FUNCTIONS
--------------------------------------------
-- Create a database that we will use to store our powerup information along with score/gems etc
-- We then create some convenience functions for loading and saving to our databases.
local function setupDatabase()
	local dbPath = system.pathForFile("appInfo.db3", system.DocumentsDirectory)
	local db = sqlite3.open(dbPath)

	local playerSetup = [[
	    CREATE TABLE playerInfo(id INTEGER PRIMARY KEY autoincrement, highscore, gems, power1, power2, power3);
	    INSERT INTO playerInfo VALUES(NULL, '0', ']] .. M.gems ..[[', ']] .. M.powerupInfo[1] .. [[', ']] .. M.powerupInfo[2] .. [[', ']] .. M.powerupInfo[3] .. [[');
	]]
	db:exec(playerSetup)

	local optionsSetup = [[
		CREATE TABLE optionsInfo(id INTEGER PRIMARY KEY autoincrement, sensitivity, soundOn );
	    INSERT INTO optionsInfo VALUES(NULL, ']] .. M.sensitivity .. [[', 'true');
	]]
	db:exec(optionsSetup)
	db:close()
end
setupDatabase()

M.loadPlayerInfo = function()
	local dbPath = system.pathForFile("appInfo.db3", system.DocumentsDirectory)
	local db = sqlite3.open(dbPath)
	for row in db:nrows("SELECT * FROM playerInfo WHERE id = 1") do
		M.highscore = tonumber(row.highscore)
		M.gems = tonumber(row.gems)
		M.powerupInfo[1] = tonumber(row.power1)
		M.powerupInfo[2] = tonumber(row.power2)
		M.powerupInfo[3] = tonumber(row.power3)
	end
	db:close()
end

M.savePlayerInfo = function()
	local dbPath = system.pathForFile("appInfo.db3", system.DocumentsDirectory)
	local db = sqlite3.open(dbPath)
	local update = "UPDATE playerInfo SET highscore='" .. M.highscore .."', gems='" .. M.gems .. "', power1='" .. M.powerupInfo[1] .."', power2='" .. M.powerupInfo[2] .."', power3='" .. M.powerupInfo[3] .."' WHERE id=1"
    db:exec(update)
	db:close()
end

M.loadOptionsInfo = function()
	local dbPath = system.pathForFile("appInfo.db3", system.DocumentsDirectory)
	local db = sqlite3.open(dbPath)
	for row in db:nrows("SELECT * FROM optionsInfo WHERE id = 1") do	
		M.sensitivity = row.sensitivity
		if row.soundOn == "false" then M.soundOn = false
		elseif row.soundOn == "true" then M.soundOn = true end 
	end
	db:close()
end

M.saveOptionsInfo = function()
	local dbPath = system.pathForFile("appInfo.db3", system.DocumentsDirectory)
	local db = sqlite3.open(dbPath)
	local updateState = M.soundOn 
	if updateState == true then updateState = "true"
	elseif updateState == false then updateState = "false" end 
	local update = "UPDATE optionsInfo SET sensitivity='" .. M.sensitivity .."', soundOn='" .. updateState .. "' WHERE id=1"
	db:exec(update)
	db:close()
end



--------------------------------------------
-- SOUND - PLAY/LOAD FUNCTIONS
--------------------------------------------
local sounds = {}
sounds["select"] = audio.loadSound("sounds/ball.mp3")
sounds["ball"] = audio.loadSound("sounds/ball.mp3")
sounds["brick"] = audio.loadSound("sounds/brick.mp3")
sounds["hit"] = audio.loadSound("sounds/hit.mp3")
sounds["laser"] = audio.loadSound("sounds/laser.mp3")
sounds["powerup"] = audio.loadSound("sounds/powerup.mp3")
sounds["sfx1"] = audio.loadSound("sounds/sfx1.mp3")
sounds["snare"] = audio.loadSound("sounds/snare.mp3")
sounds["dead"] = audio.loadSound("sounds/dead.mp3")
sounds["start"] = audio.loadSound("sounds/start2.mp3")
M.playSound = function(name) --Just pass a name to it. e.g. "select"
	if sounds[name] ~= nil and M.soundOn == true then 
		audio.play(sounds[name])
	end
end





--Return it now
return M