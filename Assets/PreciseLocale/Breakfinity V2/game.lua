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
local composer = require( "composer" )
local scene = composer.newScene()

-----------------------------------------------
--*** Set up our variables etc ***
-----------------------------------------------
-- Some handy maths vars
local _W = display.contentWidth
local _H = display.contentHeight
local mF = math.floor

-- Load our globals, or "utils" file
local utils = require("helpers.globals")
local menus = require("helpers.menus")
local levelInfo = require("helpers.levels")
local physics = require("physics")
physics.start()
-- physics.setDrawMode("hybrid")
physics.setDrawMode("normal")
physics.setGravity(0, 0)

-- Display Groups
local screenGroup
local backGroup
local gameGroup
local blockGroup
local uiGroup

-- Game settings
local backgrounds = {  -- A list of backgrounds to be used throughout the game
    {type="image", filename="images/background1Repeat.png"},
    {type="image", filename="images/background2Repeat.png"},
    {type="image", filename="images/background3Repeat.png"}
}
local gemsToPlace = 1  -- The number of gems each level has
local hasDefenseShield = false  -- If this is true the game will start with a defense shield
local laserFrequency = 0.3 -- How often a laser is shot in seconds
local laserSpeed = 500 -- The distance the laser moves each second
local levelFallClip = 2 -- The amount of pixels the level will fall at a time.  Increasing this will not speed up the falling, only increase the jumps
local levelFallSpeed = 5 -- The distance the level falls per second
local maxAngle = 70 -- The max angle the ball can travel at
local minAngle = 10 -- The min angle prevents the ball from moving directly vertical
local paddlePos = { _W*0.5, 384 }  -- The x and y starting position of the paddle
local powerupChance =  0.07  -- Chance between 0 and 1 of getting a powerup
local powerupFallSpeed = 100  -- The distance powerup collectables fall per second
local powerupLength = 5  -- The length of time a powerup will last at level one
local scrollSpeed = 10 -- Distance the background moves per second
local speedIncrease = 5 -- The ball's speed increase each time it hits the paddle
local startSpeed = 300 -- The speed the ball moves at the start of the game

-- Game variables
local canCollision = false -- If this is false the ball will not collide with blocks
local currentAngle = 34 -- Keeps track of the ball's current angle
local currentLevel = 1  -- The current level number
local currentPowerup = 0  -- The player's current powerup (0 none, 1 is laser, 2 is big ball, 3 is extend)
local currentScore = 0 -- Keeps track of our score
local currentSpeed -- Keeps track of the ball's current speed
local gameHasStarted = false  -- This is false when we haven't fired the ball
local gameIsActive = false  -- This is false when we haven't pressed play. Set to false when we pause etc
local gameTime = 0  -- The time in seconds since the start of the game for determining deltaTime in the game tick
local isMovingUp = true  -- The direction the ball is moving vertically
local isTransitioningLevel = false  -- If this is true, a new level is created when the ball passes 60% of the screen
local laserTimeRemaining  -- The time remaining until a laser is shot
local levelFallPosition = 0  -- How far the level has fallen currently
local powerupRemaining = 0  -- The amount of time until the powerup expires
local useLevel = {}  -- The level copied from levels.lua

-- Display objects
local background  -- The scrolling background of the level
local bricks = {}  -- The bricks
local defenseShields = {}  -- The defense shield blocks
local gemText  -- The text showing the number of gems the player has
local highscoreText  -- The text showing the player's score
local lasers = {}  -- The lasers
local paddleBall  -- The ball
local paddleImage  -- The paddle
local powerups = {}  -- The falling powerups
local scoredGems = {}  -- The gem which appears when you break a gem block
local settingsBtn  -- The settings button

-- Sprites/variables for the initial objects we display
local madeByTrans   --Transition that resizes the breakfinity logo
local madeByText, logo
local playBtn, playText

-- Pre-declaration of functions
local startGame


-----------------------------------------------
--*** OTHER FUNCTIONS ***
-----------------------------------------------
-- Create a new level
local function createLevel( level )
	local brickWidth = (_W-18)/8
	local startY = 58
	local tempLevel = levelInfo.levels[level][ math.random(1,#levelInfo.levels[level]) ]
	local useColour = levelInfo.colours[ math.random(1,#levelInfo.colours) ]

    -- Copy the contents of the level into useLevel
    useLevel = {}
	for i=1, #tempLevel[1] do
		for j=1, #tempLevel do
            if not useLevel[j] then
                useLevel[j] = {}
            end
			local num = tempLevel[j][i]
            useLevel[j][i] = num

            -- Create a brick if the current value in the level array isn't 0
			if num ~= 0 then
				local col
                if num == 3 then
                    -- The number 3 is used in the level array to make white bricks which take three hits
                    col = {1, 1, 1, 1}
                else
                    col = useColour[num]
                end
		
				bricks[#bricks+1] = display.newSprite(utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex(levelInfo.bricks.normal), utils.spriteSheetData:getFrameIndex(levelInfo.bricks.gem), utils.spriteSheetData:getFrameIndex(levelInfo.bricks.light)}})
				bricks[#bricks].width = brickWidth    --To make sure its always 8 bricks across the whole screeen
				bricks[#bricks].x = mF( bricks[#bricks].width/2 ) + ((i-1)*(brickWidth+2)) + 3
				bricks[#bricks].y = startY + ((j-1)*(bricks[#bricks].height+2))
                bricks[#bricks].originalY = startY + ((j-1)*(bricks[#bricks].height+2))
				bricks[#bricks]:setFillColor(col[1], col[2], col[3], col[4])

                -- Save the colour of the brick so it can be used for making lighter or darker later
                bricks[#bricks].r = col[1]
                bricks[#bricks].g = col[2]
                bricks[#bricks].b = col[3]
                bricks[#bricks].a = col[4]

				bricks[#bricks].hasGem = false
				bricks[#bricks].name = "brick"
                bricks[#bricks].col = i
                bricks[#bricks].row = j

                if num == 3 then
                    -- White bricks take three hits
                    bricks[#bricks].lives = 3
                    bricks[#bricks].isWhite = true
                else
                    bricks[#bricks].lives = 1
                    bricks[#bricks].isWhite = false
                end

                bricks[#bricks].wasHit = false
                bricks[#bricks].hurtThisFrame = false
                bricks[#bricks].hitByLaser = false
				bricks[#bricks].fill.effect = "filter.exposure"  --Force some exposure onto the brick to make it more exciting looking!
				bricks[#bricks].fill.effect.exposure = 0.74
				physics.addBody( bricks[#bricks], {isSensor=true} )  --So that our collision function registers the ball touching it.
				blockGroup:insert(bricks[#bricks])
			end
		end
	end

    -- Try to create a gem
    local gemsPlaced = 0
    while true do
        -- Choose a random brick
        local randomBrickIndex = math.random(1, #bricks)

        -- Only place the gem if the brick isn't white or already gem
        if not bricks[randomBrickIndex].isWhite and not bricks[randomBrickIndex].hasGem then
            bricks[randomBrickIndex].hasGem = true
            bricks[randomBrickIndex]:setFrame(2)
            bricks[randomBrickIndex].width = brickWidth
            bricks[randomBrickIndex].lives = 3 -- Gem bricks need to be hit three times

            gemsPlaced = gemsPlaced + 1

            if gemsPlaced == gemsToPlace then
                break
            end
        end
    end

    -- Slide the level from the top of the screen
    transition.to(blockGroup, {y=0, time=200, onComplete=function()
        canCollision = true
    end})

    -- Play a level start sound
    utils.playSound("start")

    -- Change the background depending on the level
    local tempRepeatY = background.fill.y -- Store the background's Y position to stop the background from jumping
    display.setDefault( "textureWrapY", "repeat" )
    background.fill = backgrounds[math.floor((level - 1) / (#levelInfo.levels / #backgrounds)) + 1]
    background.fill.scaleY = 0.6
    background.fill.y = tempRepeatY
    display.setDefault( "textureWrapY", "clampToEdge" ) --Reset it afterwards to avoid bugs.
end

-- Resume the game
local function resumeGame(event)
	gameIsActive = true
end

-- Called when the game is over
local function gameOver()
    gameIsActive = false

    -- Play a gameover sound
    utils.playSound("dead")

    -- Save the new highscore
    if currentScore > utils.highscore then
        utils.highscore = currentScore
        utils.savePlayerInfo()
    end

    -- Remove any powerups that may have been active.
    if currentPowerup ~= 0 then 
        if currentPowerup == 3 then 
            paddleImage:setFrame(1)
            physics.removeBody( paddleImage )
            physics.addBody(paddleImage, {isSensor=true}) 
        elseif currentPowerup == 2 then 
            paddleBall.width = 16
            paddleBall.height = 16
            physics.removeBody( paddleBall )
            physics.addBody(paddleBall, {isSensor=true}) 
        end
        powerupRemaining = 0
        currentPowerup = 0
    end
    for i=#lasers, 1, -1 do 
        if lasers[i] ~= nil then display.remove(lasers[i]); lasers[i] = nil; end
    end
    for i=#powerups, 1, -1 do 
        if powerups[i] ~= nil then display.remove(powerups[i]); powerups[i] = nil; end
    end
    lasers={}
    powerups={}

    -- Show the gameover menu
    menus.showMenu("gameOver", {from="game", highscoreText=highscoreText, gemText=gemText, score=currentScore, highscore=utils.highscore, onComplete=startGame})
end

-- Start a new game
function startGame( defenseShield, newBall )
	if madeByTrans then transition.cancel(madeByTrans); madeByTrans=nil; end
	if madeByText ~= nil then display.remove(madeByText); madeByText=nil; end
	if logo ~= nil then display.remove(logo); logo=nil; end
	gameIsActive = true
	gameHasStarted = false
    

	-- Reset positions and scores etc. But only if we haven't bought a ball.
    if newBall == nil then 
        -- Reset scores etc
        currentScore = 0
        levelFallPosition = 0 
    	highscoreText.text = "0"
        utils.ballCost = 1
        currentLevel = 1

        -- Remove bricks and create a level.
        for i=#bricks, 1, -1 do
            if bricks[i] ~= nil then display.remove( bricks[i] ); bricks[i]=nil; end
        end
        createLevel( 1 )

        -- Set our variable if needed
        if defenseShield ~= nil and defenseShield == true then 
            hasDefenseShield = true 
        else
            hasDefenseShield = false
        end

        -- Remove old defense shield if needed.
        for i=#defenseShields, 1, - 1 do 
            if defenseShields[i] ~= nil then display.remove( defenseShields[i] ); defenseShields[i]=nil; end
        end

        -- Create a defense shield if hasDefenseShield is true
        if hasDefenseShield then
            for i=1,19 do
                -- Create 19 blocks across the screen
                defenseShields[#defenseShields+1] = display.newSprite(utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("brick_greyscale_defense")}})
                defenseShields[#defenseShields].x = _W / 19 * i - defenseShields[#defenseShields].width / 2 - 1
                defenseShields[#defenseShields].y = 460  --_H - 20
                defenseShields[#defenseShields].name = "defenseShield"
                physics.addBody(defenseShields[#defenseShields], {isSensor=true})
            end 
        end
    end
    gemText.text = utils.gems
    currentSpeed = startSpeed
    currentAngle = 34
    isMovingUp = true
    paddleImage.x = paddlePos[1]
	paddleImage.y = paddlePos[2]
	paddleBall.x = _W / 2
	paddleBall.y = paddleImage.y - paddleImage.height
end

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
				if playBtn ~= nil then display.remove( playBtn ); playBtn = nil; end
				if playText ~= nil then display.remove( playText ); playText = nil; end

				if id == "play" then
					menus.showMenu("play", {onComplete=startGame, highscoreText=highscoreText, gemText=gemText})

				elseif id == "settings" then
					if gameIsActive == true then
						gameIsActive = false
						menus.showMenu("settings", {from="game", gemText=gemText, onComplete=resumeGame})
					else
						menus.showMenu("settings", {from="none", gemText=gemText, onComplete=startGame})
					end
				end
			end
		end
	end
	return true
end

-- Shake the screen
local function shakeScreen()
    local function moveScreen()
        -- Randomly move the screen somewhere
        screenGroup.x = math.random(1, 10) - 5
        screenGroup.y = math.random(1, 10) - 5
    end

    local function resetScreen()
        -- Move the screen to its original position
        screenGroup.x = 0
        screenGroup.y = 0
    end

    -- Run moveScreen 15 times over 150ms
    for i=1, 15 do
        timer.performWithDelay(10*(i-1), moveScreen)
    end

    -- After 150ms move the screen back to its original position
    timer.performWithDelay(150, resetScreen)
end

-- Called when a brick is hit by a laser or the ball
local function hitBrick(brick)
    if not brick.hurtThisFrame then
        brick.hurtThisFrame = true

        -- Take away two lives if the ball is big
        if currentPowerup == 2 then
            brick.lives = brick.lives - 2
        else
            brick.lives = brick.lives - 1
        end

        if brick.lives <= 0 then
            brick.wasHit = true
        end

        -- Play a gem brick hit sound
        if brick.hasGem == true then 
            utils.playSound("hit")
        end
    end
end

-- Called to hit the surrounding bricks if the ball is a big ball
local function hitSurrounding(brick)
    if currentPowerup == 2 then
        shakeScreen()
        for i=1, #bricks do
            if (bricks[i].col == brick.col - 1 and bricks[i].row == brick.row) or
                (bricks[i].col == brick.col and bricks[i].row == brick.row - 1) or
                (bricks[i].col == brick.col + 1 and bricks[i].row == brick.row) or
                (bricks[i].col == brick.col and bricks[i].row == brick.row + 1) or
                (bricks[i].col == brick.col + 1 and bricks[i].row == brick.row + 1) or
                (bricks[i].col == brick.col - 1 and bricks[i].row == brick.row + 1) or
                (bricks[i].col == brick.col + 1 and bricks[i].row == brick.row - 1) or
                (bricks[i].col == brick.col - 1 and bricks[i].row == brick.row - 1) then
                hitBrick(bricks[i])
            end
        end
    end
end

-- Used for moving the paddle around
local storedX = 0
local function paddleTouched(event)
    local t = event.target

    if event.phase == "began" and gameIsActive == true then
        display.getCurrentStage():setFocus( t )
        t.isFocus = true
        storedX = paddleImage.x

    elseif t.isFocus then     	
        if event.phase == "moved" and gameIsActive == true then

        	--Move the paddle and and limit it to the screen bounds
            local sensitivityValue = 1
            if utils.sensitivity == "medium" then sensitivityValue = 1.4
            elseif utils.sensitivity == "high" then sensitivityValue = 1.8 end 

            paddleImage.x = ((event.x - event.xStart)*sensitivityValue) + storedX

            if paddleImage.x - paddleImage.width/2 < 0 then paddleImage.x = paddleImage.width/2
            elseif paddleImage.x + paddleImage.width/2 > _W then paddleImage.x = _W - paddleImage.width/2 end

            --If the game hasn't started move the ball too.
            if gameHasStarted == false then
            	paddleBall.x = paddleImage.x
            end

        elseif event.phase == "ended" or event.phase == "cancelled" then
            display.getCurrentStage():setFocus( nil )
            t.isFocus = false

            --Fire the ball if we haven't already done so.
            if gameHasStarted == false then
            	gameHasStarted = true
            end
        end
    end
    return true
end

-- Called when two objects collide
local function onCollision(event)
	if gameIsActive == true then
		if event.phase == "began" then
			local name1 = event.object1.name
			local name2 = event.object2.name

			if name1 == "paddle" and name2 == "powerup" or name1 == "powerup" and name2 == "paddle" then
                -- If the paddle and a powerup collide
				local powerup
				if name1 == "powerup" then powerup = event.object1
				elseif name2 == "powerup" then powerup = event.object2 end

                -- Remove the powerup which was picked up
                for i=1, #powerups do
                    if powerups[i] == powerup then
                        currentPowerup = powerup.frame
                        powerupRemaining = utils.powerupInfo[currentPowerup] * powerupLength -- Give the user powerupLength seconds per level for this powerup
                        powerups[i]:removeSelf()
                        table.remove(powerups, i)

                        -- Reset its physics body just incase its no longer an extended paddle
                        timer.performWithDelay(30, function()
                            physics.removeBody( paddleImage )
                            physics.addBody(paddleImage, {isSensor=true}) 
                            physics.removeBody( paddleBall )
                            physics.addBody(paddleBall, {isSensor=true}) 
                        end, 1)

                        break
                    end
                end

                -- Play a sound when we pick it up. You may want different sounds depending on the powerup you pick up, so you can do that here.
                utils.playSound("powerup")
			elseif name1 == "brick" and name2 == "laser" or name1 == "laser" and name2 == "brick" then
                -- If a brick and a laser collide

                local laser
                if name1 == "laser" then laser = event.object1
                elseif name2 == "laser" then laser = event.object2 end

                local brick
                if name1 == "brick" then brick = event.object1
                elseif name2 == "brick" then brick = event.object2 end

                -- Remove the laser/brick which hit
                if canCollision then
                    for i=1, #bricks do
                        if bricks[i] == brick then
                            brick.hitByLaser = true
                            hitBrick(brick)
                            break
                        end
                    end

                    for i=1, #lasers do
                        if lasers[i] == laser then
                            lasers[i]:removeSelf()
                            table.remove(lasers, i)
                            break
                        end
                    end
                end
            elseif name1 == "ball" and name2 == "defenseShield" or name1 == "defenseShield" and name2 == "ball" then
                -- If the ball hits the defense shield
                local defenseShield
                if name1 == "defenseShield" then defenseShield = event.object1
                elseif name2 == "defenseShield" then defenseShield = event.object2 end

                -- Remove the defenseShield which was hit and make the ball move up
                if not isMovingUp then
                    for i=1, #defenseShields do
                        if defenseShields[i] == defenseShield then
                            isMovingUp = true
                            defenseShields[i]:removeSelf()
                            table.remove(defenseShields, i)
                            utils.playSound("ball") -- Play a sound 
                            break
                        end
                    end
                end
            end
		end
	end
end

-- Called each frame
-- Unless you want to change powerups, you won't really need to touch this function much
local function gameTick(event)
    -- Work out the amount of time since the last gameTick for moving objects
    if gameTime == 0 then
        gameTime = event.time / 1000
    end

    local deltaTime = event.time / 1000 - gameTime
    gameTime = event.time / 1000

	if gameIsActive == true then
		if gameHasStarted == true then
            local amountRemainingToMove = currentSpeed

            -- We move the ball in a loop to make sure the ball doesn't go through blocks at high speeds
            while amountRemainingToMove ~= 0 do
                local amountToMove = 50

                -- Work out how much to move the ball this iteration
                if amountRemainingToMove >= 50 then
                    amountToMove = 50
                    amountRemainingToMove = amountRemainingToMove - 50
                elseif amountRemainingToMove > 0 then
                    amountToMove = amountRemainingToMove
                    amountRemainingToMove = 0
                end

                -- Move the ball in the current direction
                if isMovingUp then
                    paddleBall.x = paddleBall.x - math.sin(-currentAngle*(math.pi/180)) * (amountToMove * deltaTime)
                    paddleBall.y = paddleBall.y - math.cos(-currentAngle*(math.pi/180)) * (amountToMove * deltaTime)
                else
                    paddleBall.x = paddleBall.x + math.sin(currentAngle*(math.pi/180)) * (amountToMove * deltaTime)
                    paddleBall.y = paddleBall.y + math.cos(currentAngle*(math.pi/180)) * (amountToMove * deltaTime)
                end

    			--Check to see if the ball needs to bounce of the walls or if we have lost a life
    			if paddleBall.x + paddleBall.width/2 >= _W or mF(paddleBall.x - paddleBall.width/2) <= 0 then
                    -- If the ball hits the left or right wall, invert the angle
                    -- We move the ball to the edge of the screen to stop the ball from visually going slightly off-screen
    				if paddleBall.x + paddleBall.width / 2 > _W then
                        paddleBall.x = _W - paddleBall.width / 2
                        currentAngle = -math.abs(currentAngle)
    				elseif paddleBall.x - paddleBall.width / 2 < 0 then
                        paddleBall.x = 0 + paddleBall.width / 2
                        currentAngle = math.abs(currentAngle)
                    end
    			end

    			if paddleBall.y - paddleBall.height / 2 <= 0 then
                    -- If the ball hits the top of the screen, tell the ball to move down
    				isMovingUp = false
                    canCollision = false

                    -- Complete the level
                    transition.to(blockGroup, {y=_H, time=200, onComplete=function(event)
                        blockGroup.y = -_W
                        levelFallPosition = 0
                        for i=#bricks, 1, -1 do
                            bricks[i]:removeSelf()
                            table.remove(bricks, i)
                        end
                        currentLevel = currentLevel + 1
                        useLevel = {}
                        isTransitioningLevel = true
                    end})
    			elseif paddleBall.y > _H + paddleBall.height / 2 then
                    -- If the ball hits the bottom of the screen, game over
    				gameOver()
                    shakeScreen()
    			end

    			--Now check to see if its colliding with the paddle, but only when the ball is travelling down
    			--If it does collect check the x position to see if we should change the dx
                if not isMovingUp then
    				if paddleBall.x + paddleBall.width / 2 >= (paddleImage.x - paddleImage.width/2) and paddleBall.x - paddleBall.width / 2 <= (paddleImage.x + paddleImage.width/2)  then
    					if paddleBall.y + paddleBall.height / 2 >= (paddleImage.y - paddleImage.height/2) and paddleBall.y + paddleBall.height / 2 <= paddleImage.y then
    						isMovingUp = true
                           
                            -- Play a sound
                            utils.playSound("ball")

                            -- Work out the balls new angle
                            currentAngle = (paddleBall.x-paddleImage.x) / (paddleImage.width/2) * maxAngle

                            -- Make sure the ball hasn't gone over the max angle
                            if math.abs(currentAngle) > maxAngle then
                                if currentAngle > 0 then
                                    currentAngle = maxAngle
                                else
                                    currentAngle = -maxAngle
                                end
                            end

                            -- Make sure the ball hasn't gone under the minimum angle
                            if math.abs(currentAngle) < minAngle then
                                if currentAngle > 0 then
                                    currentAngle = minAngle
                                else
                                    currentAngle = -minAngle
                                end
                            end

                            -- Increase the speed
                            currentSpeed = currentSpeed + speedIncrease
    					end
    				end
    			end

                -- Check if the ball is colliding with a brick
                if canCollision then
                    for i = #bricks, 1, -1 do
                        -- If the ball is touching the x of the brick
                        if bricks[i].x - bricks[i].width / 2 < paddleBall.x + paddleBall.width / 2 and bricks[i].x + bricks[i].width / 2 > paddleBall.x - paddleBall.width / 2 then
                            -- Check if the top of the ball is inside the brick, but the middle isn't
                            if bricks[i].y + bricks[i].height / 2 > paddleBall.y - paddleBall.height / 2 and bricks[i].y - bricks[i].height / 2 < paddleBall.y - paddleBall.height/2 and
                            bricks[i].y + bricks[i].height / 2 < paddleBall.y - paddleBall.height / 4 then
                                -- The ball should only be bounced downwards if there is no brick below this one
                                if (useLevel[bricks[i].row + 1] and useLevel[bricks[i].row + 1][bricks[i].col] and useLevel[bricks[i].row+1][bricks[i].col] == 0) or not useLevel[bricks[i].row + 1] then
                                    -- Bounce the ball downwards
                                    isMovingUp = false
                                    hitBrick(bricks[i])
                                    hitSurrounding(bricks[i])
                                end
                            -- Check if the bottom of the ball is inside the brick, but the middle isn't
                            elseif bricks[i].y - bricks[i].height / 2 < paddleBall.y + paddleBall.height / 2 and bricks[i].y + bricks[i].height / 2 > paddleBall.y + paddleBall.height/2 and
                            bricks[i].y - bricks[i].height / 2 > paddleBall.y + paddleBall.height / 4 then
                                -- The ball should only be bounced upwards if there is no brick above this one
                                if (useLevel[bricks[i].row - 1] and useLevel[bricks[i].row - 1][bricks[i].col] and useLevel[bricks[i].row-1][bricks[i].col] == 0) or not useLevel[bricks[i].row - 1] then
                                    -- Bounce the ball upwards
                                    isMovingUp = true
                                    hitBrick(bricks[i])
                                    hitSurrounding(bricks[i])
                                end
                            end
                        end

                        -- If the ball is touching the y of the brick
                        if bricks[i].y - bricks[i].height / 2 < paddleBall.y + paddleBall.height / 2 and bricks[i].y + bricks[i].height / 2 > paddleBall.y - paddleBall.height / 2 then
                            -- Check the left side of the ball is inside a brick but the middle isn't
                            if paddleBall.x - paddleBall.width / 2 < bricks[i].x + bricks[i].width / 2 and
                               paddleBall.x - paddleBall.width / 4 > bricks[i].x + bricks[i].width / 2 then
                                -- The ball should only bounce right if there isn't a brick to the right of this one
                                if (useLevel[bricks[i].row] and useLevel[bricks[i].row][bricks[i].col + 1] and useLevel[bricks[i].row][bricks[i].col + 1] == 0) then
                                    currentAngle = math.abs(currentAngle)
                                    hitBrick(bricks[i])
                                    hitSurrounding(bricks[i])
                                end
                            -- Check the right side of the ball is inside a brick but the left side isn't
                            elseif paddleBall.x + paddleBall.width / 2 > bricks[i].x - bricks[i].width / 2 and
                                   paddleBall.x + paddleBall.width / 4 < bricks[i].x - bricks[i].width / 2 then
                                if (useLevel[bricks[i].row] and useLevel[bricks[i].row][bricks[i].col - 1] and useLevel[bricks[i].row][bricks[i].col - 1] == 0) then
                                    currentAngle = -math.abs(currentAngle)
                                    hitBrick(bricks[i])
                                    hitSurrounding(bricks[i])
                                end
                            end
                        end
                    end
                end

                -- Remove all bricks which were hit by the ball
                local playedSound = false
                for i = #bricks, 1, -1 do
                    if bricks[i].wasHit then
                        local brokenRow = bricks[i].row
                        local brokenCol = bricks[i].col
                        local brokenX = bricks[i].x
                        local brokenY = bricks[i].y
                        local brokenWasGem = bricks[i].hasGem
                        local brokenByLaser = bricks[i].hitByLaser

                        -- Play a soun
                        if playedSound == false and brokenWasGem == false then 
                            utils.playSound("snare")
                            playedSound = true 
                        end

                        -- Remove the brick
                        useLevel[bricks[i].row][bricks[i].col] = 0
                        bricks[i]:removeSelf()
                        table.remove(bricks, i)

                        -- Increase the score
                        currentScore = currentScore + 10
                        highscoreText.text = currentScore

                        -- Animate the surrounding bricks
                        local function transitionOriginalScale(target)
                            transition.to(target, {xScale=1, yScale=1, time=70})
                        end

                        -- Only animate the surrounding bricks if the 
                        if not brokenByLaser then
                            for j = 1, #bricks do
                                if (bricks[j].col == brokenCol-1 and bricks[j].row == brokenRow-1) or
                                    (bricks[j].col == brokenCol-1 and bricks[j].row == brokenRow+1) or
                                    (bricks[j].col == brokenCol+1 and bricks[j].row == brokenRow+1) or
                                    (bricks[j].col == brokenCol+1 and bricks[j].row == brokenRow-1) or
                                    (bricks[j].col == brokenCol-2 and bricks[j].row == brokenRow) or
                                    (bricks[j].col == brokenCol and bricks[j].row == brokenRow+2) or
                                    (bricks[j].col == brokenCol+2 and bricks[j].row == brokenRow) or
                                    (bricks[j].col == brokenCol and bricks[j].row == brokenRow-2) then
                                    transition.to(bricks[j], {xScale=1.1, yScale=1.1, time=30, onComplete=transitionOriginalScale})
                                end

                                if (bricks[j].col == brokenCol-1 and bricks[j].row == brokenRow) or
                                    (bricks[j].col == brokenCol and bricks[j].row == brokenRow+1) or
                                    (bricks[j].col == brokenCol+1 and bricks[j].row == brokenRow) or
                                    (bricks[j].col == brokenCol and bricks[j].row == brokenRow-1) then
                                    transition.to(bricks[j], {xScale=1.2, yScale=1.2, time=30, onComplete=transitionOriginalScale})
                                end
                            end
                        end

                        -- Create a powerup randomly
                        if not brokenWasGem and math.random() < powerupChance then
                            powerups[#powerups + 1] = display.newSprite(utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex(levelInfo.powerups.laser), utils.spriteSheetData:getFrameIndex(levelInfo.powerups.bomb), utils.spriteSheetData:getFrameIndex(levelInfo.powerups.extend)}})
                            powerups[#powerups]:setFrame(math.random(1, 3))
                            powerups[#powerups].x = brokenX + blockGroup.x
                            powerups[#powerups].y = brokenY
                            powerups[#powerups].name = "powerup"
                            physics.addBody(powerups[#powerups], {isSensor=true}) -- Give this a sensor so we can detect its collision with the paddle
                        end

                        -- Give the player gem
                        if brokenWasGem then
                            utils.gems = utils.gems + 1
                            utils.savePlayerInfo()
                            gemText.text = utils.gems
                            scoredGems[#scoredGems + 1] = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("gem_large")}} )
                            scoredGems[#scoredGems].x = brokenX
                            scoredGems[#scoredGems].y = brokenY
                            scoredGems[#scoredGems].startX = scoredGems[#scoredGems].x
                            scoredGems[#scoredGems].startY = scoredGems[#scoredGems].y
                            scoredGems[#scoredGems].currentLerp = 0
                            scoredGems[#scoredGems].xScale = 0.01
                            scoredGems[#scoredGems].YScale = 0.01
                        end
                    end
                end
    		end

            -- Move the level down
            levelFallPosition = levelFallPosition + levelFallSpeed * deltaTime
            for i = 1, #bricks do
                -- The bricks will only move two pixels at a time
                bricks[i].y = bricks[i].originalY + (math.floor(levelFallPosition/levelFallClip)*levelFallClip)

                -- Reset hurtThisFrame
                bricks[i].hurtThisFrame = false
            end

            -- Create a new level if the ball has passed 60% of the screen
            if isTransitioningLevel then
                if paddleBall.y > _H * 0.6 then
                    isTransitioningLevel = false
                    createLevel(((currentLevel-1)%#levelInfo.levels)+1)
                end
            end

            -- Flash damaged gem/white blocks
            for i=1, #bricks do
                local brick = bricks[i]

                -- Work out the dark and light versions of the brick
                local colourAmount = 0.2
                local lightR = brick.r + colourAmount
                local lightG = brick.g + colourAmount
                local lightB = brick.b + colourAmount

                local darkR = brick.r - colourAmount
                local darkG = brick.g - colourAmount
                local darkB = brick.b - colourAmount

                if lightR > 1 then
                    lightR = 1
                end

                if lightG > 1 then
                    lightG = 1
                end

                if lightB > 1 then
                    lightB = 1
                end

                -- Flash the brick depending on the lives remaining and the brick type
                if brick.hasGem and brick.lives == 2 and gameTime % 0.5 < 0.25 then
                    brick:setFillColor(lightR, lightG, lightB, brick.a)
                elseif brick.hasGem and brick.lives == 1 and gameTime % 0.2 < 0.1 then
                    brick:setFillColor(lightR, lightG, lightB, brick.a)
                elseif brick.hasGem then
                    brick:setFillColor(brick.r, brick.g, brick.b, brick.a)
                elseif brick.isWhite and brick.lives == 2 and gameTime % 0.5 < 0.25 then
                    brick:setFillColor(darkR, darkG, darkB, brick.a)
                elseif brick.isWhite and brick.lives == 1 and gameTime % 0.2 < 0.1 then
                    brick:setFillColor(darkR, darkG, darkB, brick.a)
                else
                    brick:setFillColor(brick.r, brick.g, brick.b, brick.a)
                end
            end

            -- Move powerups down
            for i=#powerups, 1, -1 do
                powerups[i].y = powerups[i].y + powerupFallSpeed * deltaTime

                -- Remove offscreen powerups
                if powerups[i].y - powerups[i].height / 2 > _H then
                    powerups[i]:removeSelf()
                    table.remove(powerups, i)
                end
            end

            -- Move lasers up
            for i=#lasers, 1, -1 do
                lasers[i].y = lasers[i].y - laserSpeed * deltaTime
                lasers[i].name = "laser"
                physics.addBody(lasers[i], {isSensor=true})

                -- Remove offscreen lasers
                if lasers[i].y + lasers[i].height / 2 < 0 then
                    lasers[i]:removeSelf()
                    table.remove(lasers, i)
                end
            end

            -- Move scored gems to the gem in the top right
            for i=#scoredGems, 1, -1 do
                scoredGems[i].currentLerp = scoredGems[i].currentLerp + 2 * deltaTime
                scoredGems[i].x = scoredGems[i].startX + ((_W - 18) - scoredGems[i].startX) * scoredGems[i].currentLerp
                scoredGems[i].y = scoredGems[i].startY + (26 - scoredGems[i].startY) * scoredGems[i].currentLerp
                scoredGems[i].xScale = 1 - scoredGems[i].currentLerp
                scoredGems[i].yScale = 1 - scoredGems[i].currentLerp
                if scoredGems[i].currentLerp >= 1 then
                    scoredGems[i]:removeSelf()
                    table.remove(scoredGems, i)
                end
            end

            -- Update powerup time remaining
            if currentPowerup ~= 0 then
                powerupRemaining = powerupRemaining - deltaTime

                -- Turn off the powerup when out of time
                if powerupRemaining < 0 then
                    if currentPowerup == 3 then 
                        paddleImage:setFrame(1)
                        physics.removeBody( paddleImage )
                        physics.addBody(paddleImage, {isSensor=true}) 
                    elseif currentPowerup == 2 then 
                        paddleBall.width = 16
                        paddleBall.height = 16
                        physics.removeBody( paddleBall )
                        physics.addBody(paddleBall, {isSensor=true}) 
                    end
                    powerupRemaining = 0
                    currentPowerup = 0
                end
            end

            -- Laser powerup
            if currentPowerup == 1 then
                paddleImage:setFrame(3)
                laserTimeRemaining = laserTimeRemaining - deltaTime
                if laserTimeRemaining <= 0 then
                    laserTimeRemaining = laserFrequency

                    -- Shoot two lasers
                    -- Left
                    lasers[#lasers + 1] = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("powerup_laser")}})
                    lasers[#lasers].x = paddleImage.x - paddleImage.width / 2
                    lasers[#lasers].y = paddleImage.y

                    -- Right
                    lasers[#lasers + 1] = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("powerup_laser")}})
                    lasers[#lasers].x = paddleImage.x + paddleImage.width / 2
                    lasers[#lasers].y = paddleImage.y

                    -- Play a laser sound
                    utils.playSound("laser")
                end
            else
                laserTimeRemaining = laserFrequency
            end

            -- Big ball powerup
            if currentPowerup == 2 then
                paddleBall.width = 32
                paddleBall.height = 32
            else
                paddleBall.width = 16
                paddleBall.height = 16
            end

            -- Extended paddle powerup
            if currentPowerup == 3 then
                paddleImage:setFrame(2)
            end

            -- Show the default paddle
            if currentPowerup ~= 3 and currentPowerup ~= 1 then
                paddleImage:setFrame(1)
            end

            -- Keep the paddle within the screen bounds, as the extend powerup may move the paddle offscreen
            if paddleImage.x - paddleImage.width/2 < 0 then paddleImage.x = paddleImage.width/2
            elseif paddleImage.x + paddleImage.width/2 > _W then paddleImage.x = _W - paddleImage.width/2 end
        end
	end

	-- Update the background fill
	background.fill.y = background.fill.y - (scrollSpeed*deltaTime) * 0.01
	if background.fill.y < -1 then
      	background.fill.y = 0
    end
end




-----------------------------------------------
-- *** COMPOSER SCENE EVENT FUNCTIONS ***
------------------------------------------------
-- Called when the scene's view does not exist:
-- Create all your display objects here.
function scene:create( event )
	-- Setup our groups
	screenGroup = self.view
	backGroup = display.newGroup()
	gameGroup = display.newGroup()
	uiGroup = display.newGroup()
    blockGroup = display.newGroup()

	screenGroup:insert(backGroup)
    screenGroup:insert(blockGroup)
	screenGroup:insert(gameGroup)
	screenGroup:insert(uiGroup)

  	-- Load the relevant information from utiles
  	utils.loadPlayerInfo()
  	utils.loadOptionsInfo()


  	-- Create our display objects
  	-- Background first -We use a fill to save on memory and its easier! This has to be a power of 2. e.g. 32, 64, 128 x 126 etc.
	display.setDefault( "textureWrapY", "repeat" ) --Needed for setting our background to graphics 2.0 repeating fills
	background = display.newRect( backGroup, _W*0.5, _H*0.5, _W, _H )
	background.fill = { type="image", filename="images/background1Repeat.png" }
	background.fill.scaleY = 0.6
	background:addEventListener("touch", paddleTouched) --Give it a touch event
	display.setDefault( "textureWrapY", "clampToEdge" ) --Reset it afterwards to avoid bugs.

	local gradient1 = display.newImageRect( backGroup, "images/gradient.png", _W,128)
	gradient1.x = _W*0.5;
	gradient1.y = 64

	local gradient2 = display.newImageRect( backGroup, "images/gradient.png", _W,128)
	gradient2.x = _W*0.5;
	gradient2.y = _H-64
	gradient2.rotation=180


	-- Ui bits.
	-- We pre-declare these a few items here so that we can transition / give them touch listeners when the scene is fully transitioned in
	local gemIcon = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("gem_large")}} )
	gemIcon.x = _W-18
	gemIcon.y = 26
	uiGroup:insert(gemIcon)

	highscoreText = display.newText({parent=uiGroup, text=utils.highscore, align="left", font=utils.pixelFont, fontSize=28})
	highscoreText.anchorX = 0
	highscoreText.x = 14
	highscoreText.y = gemIcon.y

	gemText = display.newText({parent=uiGroup, text=utils.gems, align="right", font=utils.pixelFont, fontSize=28})
	gemText.anchorX = 1
	gemText.x = gemIcon.x - 20
	gemText.y = gemIcon.y

	paddleImage = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("paddle_small_normal"), utils.spriteSheetData:getFrameIndex("paddle_small_normal_wide"), utils.spriteSheetData:getFrameIndex("paddle_small_laser")}})
	paddleImage.x = paddlePos[1]
	paddleImage.y = paddlePos[2]
    paddleImage.name = "paddle"
    physics.addBody(paddleImage, {isSensor=true}) -- Give this a sensor so we can detect its collision with the powerups
	gameGroup:insert(paddleImage)

	paddleBall = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("paddle_ball")}} )
	paddleBall.x = _W / 2
	paddleBall.y = paddleImage.y - paddleImage.height
	paddleBall.radius = 8
	paddleBall.name = "ball"
	physics.addBody(paddleBall, {radius=8, isSensor=true}) -- Give this a sensor so we can detect its collision with the bricks
	gameGroup:insert(paddleBall)

	settingsBtn = display.newSprite(utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("settings")}})
	settingsBtn.x = _W*0.5
	settingsBtn.y = gemIcon.y
	settingsBtn.id = "settings"
	uiGroup:insert(settingsBtn)
	

	-- These bits are removed when we start the game.
	-- They are also only shown once at the very beginning of the app being loaded
	logo = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("logo")}} )
	logo.x = _W*0.5
	logo.y = _H-64
	uiGroup:insert(logo)

	playBtn = display.newSprite( utils.spriteSheetImage, {frames={utils.spriteSheetData:getFrameIndex("btn_wide_green")}} )
	playBtn.x = _W*0.5 + 2
	playBtn.y = mF(_H*0.45)
	playBtn.id = "play"
	uiGroup:insert(playBtn)

	playText = display.newText({parent=uiGroup, text="Play", font=utils.pixelFont, fontSize=14})
	playText.x = playBtn.x - 2
	playText.y = playBtn.y - 4

	madeByText = display.newText({parent=uiGroup, text="Created by T and G Apps", font=utils.pixelFont, fontSize=12})
	madeByText.x = _W*0.5
	madeByText.y = _H-24

end

function scene:show( event )
    local sceneGroup = self.view
    local phase = event.phase

    if ( phase == "will" ) then
        -- Called when the scene is still off screen (but is about to come on screen).

    elseif ( phase == "did" ) then
        -- Called when the scene is now on screen.
        -- Insert code here to make the scene come alive.
        -- Example: start timers, begin animation, play audio, etc.
        composer.removeHidden()  --Remove all other scenes, not really needed in this template.

        -- Give the buttons their touch
        settingsBtn:addEventListener("touch", btnTouched)
        playBtn:addEventListener("touch", btnTouched)

        -- Add a runtime enterframe listener for moving the background etc
        Runtime:addEventListener("enterFrame", gameTick)

        -- Add a runtime collision listener
        Runtime:addEventListener("collision", onCollision)


        --Transition for madeByText
        local function startTrans()
        	local function endTrans() madeByTrans = transition.to(madeByText, {time=1500, xScale=1, yScale=1, onComplete=startTrans}) end
        	madeByTrans = transition.to(madeByText, {time=1500, xScale=0.84, yScale=0.84, onComplete=endTrans})
        end
        startTrans()
    end
end

function scene:hide( event )
    local sceneGroup = self.view
    local phase = event.phase

    if ( phase == "will" ) then
        -- Called when the scene is on screen (but is about to go off screen).
        -- Insert code here to "pause" the scene.
        -- Example: stop timers, stop animation, stop audio, etc.
        if madeByTrans ~= nil then
        	transition.cancel(madeByTrans)
        	madeByTrans=nil
        end
        Runtime:removeEventListener("enterFrame", gameTick)

    elseif ( phase == "did" ) then
        -- Called immediately after scene goes off screen.
    end
end

function scene:destroy( event )
    local sceneGroup = self.view

    -- Called prior to the removal of scene's view ("sceneGroup").
    -- Insert code here to clean up the scene.
    -- Example: remove display objects, save state, etc.
end


-- Then add the listeners for the above functions
scene:addEventListener( "create", scene )
scene:addEventListener( "show", scene )
scene:addEventListener( "hide", scene )
scene:addEventListener( "destroy", scene )


return scene