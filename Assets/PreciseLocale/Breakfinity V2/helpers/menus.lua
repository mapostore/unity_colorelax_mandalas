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
local utils = require("helpers.globals")
local mF = math.floor
local _W = display.contentWidth
local _H = display.contentHeight


-- Menu group
local menuGroup 



--------------------------------------------
-- MENUS
--------------------------------------------
-- The main aim of this file is to provide us with an easy way of hiding / showing menus
-- All of the menus are called from one function at the bottom of this file. 

-- Settings Menu - The cog at the top of the screen calls this function
local function showSettings( options )
	utils.loadOptionsInfo()

	local soundState = "on"
	local from = "none"
	local soundText, sensitivityText
	if utils.soundOn == false then soundState = "off" end 
	if options ~= nil and options.from ~= nil then from = options.from end 

	local function btnTouched(event)
		local t = event.target
		local id = t.id

		if event.phase == "began" then 
			display.getCurrentStage():setFocus( t )
			t.isFocus = true
			t.alpha = 0.8
			t.xScale = 0.8
			t.yScale = 0.8
				
		elseif t.isFocus then 
			if event.phase == "ended"  then 
				display.getCurrentStage():setFocus( nil )
				t.isFocus = false
				t.alpha = 1
				t.xScale = 1
				t.yScale = 1

				--Check bounds. If we are in it then click!
				local b = t.contentBounds 
				if event.x >= b.xMin and event.x <= b.xMax and event.y >= b.yMin and event.y <= b.yMax then 
					utils.playSound("select") -- Play a click sound
					if id == "sound" then 
						if utils.soundOn == true then utils.soundOn = false; soundState = "off"
						else utils.soundOn = true; soundState = "on"; end
						soundText.text = "Sound: " .. soundState
						utils.saveOptionsInfo()

					elseif id == "sensitivity" then 
						if utils.sensitivity == "low" then utils.sensitivity = "medium"
						elseif utils.sensitivity == "medium" then utils.sensitivity = "high"
						elseif utils.sensitivity == "high" then utils.sensitivity = "low" end
						sensitivityText.text = "Sensitivity:\n" .. utils.sensitivity
						utils.saveOptionsInfo()

					elseif id == "back" then 
						if from == "game" then 
							display.remove(menuGroup); menuGroup=nil; 
							if options.onComplete ~= nil then 
								options.onComplete(true)   --Sends this event back to the game and allows it to carry on. 
							end
						else 
							if from == "none" then from = "play" end 
							M.showMenu( from, options )  --Opens up whatever menu we were previously on. 
						end
					end
				end
			end
		end
		return true
	end

	local soundBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_wide_green")}} )
	soundBtn.x = _W*0.5 + 2
	soundBtn.y = mF(_H*0.32) 
	soundBtn.id = "sound"
	menuGroup:insert(soundBtn)

	soundText = display.newText({parent=menuGroup, text="Sound: " .. soundState, font=utils.pixelFont, fontSize=12})
	soundText.x = soundBtn.x
	soundText.y = soundBtn.y - 4

	local sensitivityBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_wide_green")}} )
	sensitivityBtn.x = soundBtn.x
	sensitivityBtn.y = soundBtn.y + 64
	sensitivityBtn.id = "sensitivity"
	menuGroup:insert(sensitivityBtn)

	sensitivityText = display.newText({parent=menuGroup, text="Sensitivity:\n" .. utils.sensitivity, width=sensitivityBtn.width, font=utils.pixelFont, fontSize=12, align="center"})
	sensitivityText.x = sensitivityBtn.x
	sensitivityText.y = sensitivityBtn.y - 4

	local backBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_wide_grey")}} )
	backBtn.x = sensitivityBtn.x
	backBtn.y = sensitivityBtn.y + 64
	backBtn.id = "back"
	menuGroup:insert(backBtn)

	local backText = display.newText({parent=menuGroup, text="Back", font=utils.pixelFont, fontSize=12, align="center"})
	backText.x = backBtn.x
	backText.y = backBtn.y - 4

	soundBtn:addEventListener("touch", btnTouched)
	sensitivityBtn:addEventListener("touch", btnTouched)
	backBtn:addEventListener("touch", btnTouched)
end

-- Play Menu - Called from various places through the game
local function showPlayMenu( options ) 
	local function btnTouched(event)
		local t = event.target
		local id = t.id

		if event.phase == "began" then 
			display.getCurrentStage():setFocus( t )
			t.isFocus = true
			t.alpha = 0.8
			t.xScale = 0.8
			t.yScale = 0.8
				
		elseif t.isFocus then 
			if event.phase == "ended"  then 
				display.getCurrentStage():setFocus( nil )
				t.isFocus = false
				t.alpha = 1
				t.xScale = 1
				t.yScale = 1

				--Check bounds. If we are in it then click!
				local b = t.contentBounds 
				if event.x >= b.xMin and event.x <= b.xMax and event.y >= b.yMin and event.y <= b.yMax then 
					utils.playSound("select") -- Play a click sound
					if id == "defense" then 
						if utils.gems >= 10 then 
							local function alertPressed(event)
								if event.index == 1 then 
									if options ~= nil and options.onComplete ~= nil then 
										display.remove(menuGroup); menuGroup=nil;
										utils.gems = utils.gems - 10
										utils.savePlayerInfo()
										options.onComplete( true )  --true gives us a defense sheild to start with.
									end
								end
							end
							local alert = native.showAlert("Alert", "Defense Shields cost 10 gems, do you want to buy one?", {"Yes", "No"}, alertPressed)
						else
							local alert = native.showAlert("Alert", "We're sorry but a defense shield cost 10 gems...", {"OK"})
						end

					elseif id == "later" then
						display.remove(menuGroup); menuGroup=nil;
						if options ~= nil and options.onComplete ~= nil then 
							options.onComplete( )
						end
						utils.canShowAd = true --Allow an ad again as we have started playing the game.

					elseif id == "free" then 
						if ( utils.ads.isAdAvailable() ) and utils.canShowAd == true then
						    utils.canShowAd = false
						    utils.showAd()
						    utils.gems = utils.gems + 5
						    utils.savePlayerInfo()
						    if options.gemText ~= nil then options.gemText.text = utils.gems end 
						else
							local alert = native.showAlert("Alert", "We're sorry but there isn't a video to view right now...", {"OK"})
						end

					elseif id == "share" then 
						local function alertPressed(event)
							if event.index == 1 then 
								utils.sharing.facebookShare( utils.shareMessage )
								-- utils.gems = utils.gems + 1
								-- utils.savePlayerInfo() 
							elseif event.index == 2 then 
								utils.sharing.twitterShare( utils.shareMessage )
								-- utils.gems = utils.gems + 1
								-- utils.savePlayerInfo() 
							end
							if options.gemText ~= nil then options.gemText.text = utils.gems end 
						end
						local alert = native.showAlert("Share App", "Which platform would you like to share this app on?", {"Faceook", "Twitter", "Cancel"}, alertPressed)
						-- local alert = native.showAlert("Share App", "Which platform would you like to share this app on? You will get a gem as a thank you!", {"Faceook", "Twitter", "Cancel"}, alertPressed)

					elseif id == "upgrade" then 
						M.showMenu( "upgrade", options )
					end
				end
			end
		end
		return true
	end

	if options.highscoreText ~= nil then
		options.highscoreText.text = utils.highscore
	end

	local defenseBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_wide2_grey")}} )
	defenseBtn.x = _W*0.5 + 2
	defenseBtn.y = mF(_H*0.32) 
	defenseBtn.id = "defense"
	menuGroup:insert(defenseBtn)

	local defenseText = display.newText({parent=menuGroup, text="Activate\nDEFENSE SHIELD?", width=defenseBtn.width, font=utils.pixelFont, fontSize=12, align="center"})
	defenseText.x = defenseBtn.x - 2
	defenseText.y = defenseBtn.y - 4 

	local shareBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_small_blue")}} )
	shareBtn.x = defenseBtn.x
	shareBtn.y = defenseBtn.y + 64
	shareBtn.id = "share"
	menuGroup:insert(shareBtn)

	local freeBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_small_black")}} )
	freeBtn.x = shareBtn.x - 100
	freeBtn.y = shareBtn.y
	freeBtn.id = "free"
	menuGroup:insert(freeBtn)

	local laterBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_small_grey")}} )
	laterBtn.x = shareBtn.x + 100
	laterBtn.y = shareBtn.y
	laterBtn.id = "later"
	menuGroup:insert(laterBtn)

	local upgradeBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_normal_pink")}} )
	upgradeBtn.x = defenseBtn.x 
	upgradeBtn.y = laterBtn.y + 64
	upgradeBtn.id = "upgrade"
	menuGroup:insert(upgradeBtn)

	local shareText = display.newText({parent=menuGroup, text="SHARE!", font=utils.pixelFont, fontSize=12, align="center"})
	shareText.x = shareBtn.x - 2
	shareText.y = shareBtn.y - 4	

	local freeText = display.newText({parent=menuGroup, text="FREE!", font=utils.pixelFont, fontSize=12, align="center"})
	freeText.x = freeBtn.x - 2
	freeText.y = freeBtn.y - 4	

	local laterText = display.newText({parent=menuGroup, text="Later", font=utils.pixelFont, fontSize=12, align="center"})
	laterText.x = laterBtn.x - 2
	laterText.y = laterBtn.y - 4	

	local upgradeText = display.newText({parent=menuGroup, text="Upgrade\nStore", width=upgradeBtn.width, font=utils.pixelFont, fontSize=12, align="center"})
	upgradeText.x = upgradeBtn.x - 2
	upgradeText.y = upgradeBtn.y - 4	

	defenseBtn:addEventListener("touch", btnTouched)
	freeBtn:addEventListener("touch", btnTouched)
	shareBtn:addEventListener("touch", btnTouched)
	laterBtn:addEventListener("touch", btnTouched)
	upgradeBtn:addEventListener("touch", btnTouched)
end

-- Upgrade Menu - Only called from the play menu
local function showUpgradeMenu( options )
	local blocks = {}
	local btnTouched
	local laserBtn, extendBtn, bombBtn

	local function checkForTouchRemove()
		if utils.powerupInfo[1] >= 3 and laserBtn.hasTouchListener == true then laserBtn:removeEventListener("touch", btnTouched); laserBtn.hasTouchListener = false; end 
		if utils.powerupInfo[2] >= 3 and bombBtn.hasTouchListener == true then bombBtn:removeEventListener("touch", btnTouched); bombBtn.hasTouchListener = false; end 
		if utils.powerupInfo[3] >= 3 and extendBtn.hasTouchListener == true then extendBtn:removeEventListener("touch", btnTouched); extendBtn.hasTouchListener = false; end 
	end

	function btnTouched(event)
		local t = event.target
		local id = t.id

		if event.phase == "began" then 
			display.getCurrentStage():setFocus( t )
			t.isFocus = true
			t.alpha = 0.8
			t.xScale = 0.8
			t.yScale = 0.8
				
		elseif t.isFocus then 
			if event.phase == "ended"  then 
				display.getCurrentStage():setFocus( nil )
				t.isFocus = false
				t.alpha = 1
				t.xScale = 1
				t.yScale = 1

				--Check bounds. If we are in it then click!
				local b = t.contentBounds 
				if event.x >= b.xMin and event.x <= b.xMax and event.y >= b.yMin and event.y <= b.yMax then 
					utils.playSound("select") -- Play a click sound
					if id == "back" then 
						M.showMenu( "play", options )

					else
						if utils.gems >= t.cost then 
							local function alertPressed(event)
								if event.index == 1 then 
									utils.gems = utils.gems - t.cost 
									utils.powerupInfo[t.powerupId] = utils.powerupInfo[t.powerupId] + 1 

									local tid = t.powerupId
									for i=1, 3 do 
										if utils.powerupInfo[tid] < i then blocks[tid][i]:setFrame(2) 
										else blocks[tid][i]:setFrame(1) end
									end

									utils.savePlayerInfo()
									checkForTouchRemove()
									if options.gemText ~= nil then options.gemText.text = utils.gems end
								end
							end
							local alert = native.showAlert("Alert", "Would you like to upgrade this powerup for "..t.cost.." gems?", {"OK", "Cancel"}, alertPressed)
						else
							local alert = native.showAlert("Alert", "We're sorry, but you don't have enough gems to buy this powerup...", {"OK"})
						end
					end				
				end
			end
		end
		return true
	end

	-- Create each upgradable powerup.
	local function addUpgradeText( btn )
		--Powerups all have the same naming style for convenience, e.g. "powerup_pill_bomb", "powerup_pill_extend" etc.
		local pill = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("powerup_pill_"..btn.id )}} )
		pill.x = btn.x - 38; pill.y = btn.y - 18
		menuGroup:insert(pill)

		local id = btn.powerupId
		blocks[id] = {}
		for i=1, 3 do		
			blocks[id][i] = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("upgrade_block_lit"), utils.spriteSheetData:getFrameIndex("upgrade_block_unlit")}} )
			blocks[id][i].x = pill.x + 32 + ((i-1)*22)
			blocks[id][i].y = pill.y - 2
			if utils.powerupInfo[id] < i then blocks[id][i]:setFrame(2) end
			menuGroup:insert(blocks[id][i])
		end

		local timeText = display.newText({parent=menuGroup, text=btn.id:gsub("^%l", string.upper).." Time", font=utils.pixelFont, fontSize=10 })
		timeText.x = btn.x-3; timeText.y = btn.y + 2

		local upgradeText = display.newText({parent=menuGroup, text="Upgrade", font=utils.pixelFont, fontSize=10 })
		upgradeText.anchorX=0; upgradeText.x = btn.x-btn.width/2+8; upgradeText.y = timeText.y + 14

		local gemImage = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("gem_small")}} )
		gemImage.x = upgradeText.x + upgradeText.width + 10; gemImage.y = upgradeText.y
		menuGroup:insert(gemImage)

		local costText = display.newText({parent=menuGroup, text=btn.cost, font=utils.pixelFont, fontSize=10, align="left" })
		costText.anchorX=0; costText.x = gemImage.x+gemImage.width/2+6; costText.y = gemImage.y
	end

	laserBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_tall_blue")}} )
	laserBtn.x = mF((_W*0.5 + 2) - laserBtn.width/2 - 4)
	laserBtn.y = mF(_H*0.32) 
	laserBtn.id = "laser"
	laserBtn.cost = 50
	laserBtn.powerupId = 1
	laserBtn.hasTouchListener = true 
	menuGroup:insert(laserBtn)
	addUpgradeText( laserBtn )

	bombBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_tall_blue")}} )
	bombBtn.x = laserBtn.x
	bombBtn.y = laserBtn.y + 84
	bombBtn.id = "bomb"
	bombBtn.cost = 50
	bombBtn.powerupId = 2
	bombBtn.hasTouchListener = true 
	menuGroup:insert(bombBtn)
	addUpgradeText( bombBtn )

	extendBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_tall_blue")}} )
	extendBtn.x = mF((_W*0.5 + 2) + extendBtn.width/2 + 4)
	extendBtn.y = laserBtn.y
	extendBtn.id = "extend"
	extendBtn.cost = 50
	extendBtn.powerupId = 3
	extendBtn.hasTouchListener = true 
	menuGroup:insert(extendBtn)
	addUpgradeText( extendBtn )

	-- Then the back button..
	local backBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_tall_grey")}} )
	backBtn.x = extendBtn.x
	backBtn.y = bombBtn.y
	backBtn.id = "back"
	backBtn.cost = 50
	menuGroup:insert(backBtn)

	local backText = display.newText({text="Back", font=utils.pixelFont, fontSize=12, align="right" })
	backText.x = backBtn.x; backText.y = backBtn.y - 4
	menuGroup:insert(backText)


	-- Add touch events
	bombBtn:addEventListener("touch", btnTouched)
	laserBtn:addEventListener("touch", btnTouched)
	extendBtn:addEventListener("touch", btnTouched)
	backBtn:addEventListener("touch", btnTouched)
	checkForTouchRemove()  -- So we can't attempt to buy an already maxed item.
end

-- Game over screen - Only called from the gameOver function in game.lua
local function showGameOver( options )
	local function btnTouched(event)
		local t = event.target
		local id = t.id

		if event.phase == "began" then 
			display.getCurrentStage():setFocus( t )
			t.isFocus = true
			t.alpha = 0.8
			t.xScale = 0.8
			t.yScale = 0.8
				
		elseif t.isFocus then 
			if event.phase == "ended"  then 
				display.getCurrentStage():setFocus( nil )
				t.isFocus = false
				t.alpha = 1
				t.xScale = 1
				t.yScale = 1

				--Check bounds. If we are in it then click!
				local b = t.contentBounds 
				if event.x >= b.xMin and event.x <= b.xMax and event.y >= b.yMin and event.y <= b.yMax then 
					utils.playSound("select") -- Play a click sound
					if id == "later" then
						
					elseif id == "free" then 

					elseif id == "buy" then 
						if utils.ballCost <= utils.gems then 
							utils.gems = utils.gems - utils.ballCost
							utils.ballCost = utils.ballCost * 2
							utils.savePlayerInfo()

							display.remove(menuGroup); menuGroup=nil;	
							if options ~= nil and options.onComplete ~= nil then 
								options.onComplete( false, true ) --First boolean is defense shield, second tells the function we are resetting our ball.  
							end

						else
							local alert = native.showAlert("Alert", "We're sorry, but you don't have enough gems!", {"OK"})
						end

					elseif id == "done" then 
						M.showMenu( "play", options )
					end
				end
			end
		end
		return true
	end

	local highscoreRect = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_wide_grey")}} )
	highscoreRect.x = _W*0.5 + 2
	highscoreRect.y = mF(_H*0.32) 
	menuGroup:insert(highscoreRect)

	local highscoreTrophy = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_cup")}} )
	highscoreTrophy.x = mF(highscoreRect.x - highscoreRect.width/2 + highscoreTrophy.width/2 + 5)
	highscoreTrophy.y = highscoreRect.y - 4
	menuGroup:insert(highscoreTrophy)	

	local scoreText = display.newText({text="Score", font=utils.pixelFont, fontSize=12, align="left" })
	scoreText.anchorX = 0; scoreText.x = mF(highscoreTrophy.x + highscoreTrophy.width/2 + 4)
	scoreText.y = highscoreTrophy.y - 10
	menuGroup:insert(scoreText)

	local scoreText2 = display.newText({text=options.score, font=utils.pixelFont, fontSize=12, align="right" })
	scoreText2.anchorX = 1; scoreText2.x = mF(highscoreRect.x + highscoreRect.width/2 - 12)
	scoreText2.y = scoreText.y 
	menuGroup:insert(scoreText2)

	local bestText = display.newText({text="Best", font=utils.pixelFont, fontSize=12, align="left" })
	bestText.anchorX = 0; bestText.x = scoreText.x
	bestText.y = highscoreTrophy.y + 10
	menuGroup:insert(bestText)

	local bestText2 = display.newText({text=options.highscore, font=utils.pixelFont, fontSize=12, align="right" })
	bestText2.anchorX = 1; bestText2.x = scoreText2.x; bestText2.y = bestText.y 
	menuGroup:insert(bestText2)

	local ballBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_normal_blue")}} )
	ballBtn.x = highscoreRect.x
	ballBtn.y = highscoreRect.y + 64  
	ballBtn.id = "buy"
	ballBtn:addEventListener("touch", btnTouched)
	menuGroup:insert(ballBtn)

	local ballText1 = display.newText({text="Buy Extra", font=utils.pixelFont, fontSize=12, align="right" })
	ballText1.x = ballBtn.x-2; ballText1.y = ballBtn.y - 12
	menuGroup:insert(ballText1)

	local ballText2 = display.newText({text="Ball", font=utils.pixelFont, fontSize=12, align="right" })
	ballText2.anchorX = 0; ballText2.x = mF(ballBtn.x - ballBtn.width/2 + 17); ballText2.y = ballBtn.y + 7
	menuGroup:insert(ballText2)

	local gemImage = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("gem_small")}} )
	gemImage:scale(1.1,1.1); gemImage.x = ballBtn.x + 12; gemImage.y = ballText2.y
	menuGroup:insert(gemImage)

	local ballText3 = display.newText({text=utils.ballCost, font=utils.pixelFont, fontSize=12, align="left" })
	ballText3.anchorX = 0; ballText3.x = mF(gemImage.x + gemImage.width/2 + 8); ballText3.y = gemImage.y
	menuGroup:insert(ballText3)

	local doneBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_wide_grey")}} )
	doneBtn.x = highscoreRect.x-2
	doneBtn.y = ballBtn.y + 64  
	doneBtn.id = "done"
	doneBtn:addEventListener("touch", btnTouched)
	menuGroup:insert(doneBtn)

	local doneText = display.newText({text="Done", font=utils.pixelFont, fontSize=12, align="right" })
	doneText.x = doneBtn.x; doneText.y = doneBtn.y - 4
	menuGroup:insert(doneText)
end


-- showMenu function called from any other scene
-- This removes the current one showing and displays a new one!
M.showMenu = function( menu, options )
	if menuGroup ~= nil then display.remove( menuGroup ); menuGroup = nil; end
	menuGroup = display.newGroup() 

	if menu ~= nil then 
		if menu == "play" then showPlayMenu(options) 
		elseif menu == "settings" then showSettings(options)
		elseif menu == "upgrade" then showUpgradeMenu(options)
		elseif menu == "gameOver" then showGameOver(options)
		end
	end
end





--Return it now
return M