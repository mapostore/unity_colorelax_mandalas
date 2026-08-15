--
-- created with TexturePacker (http://www.codeandweb.com/texturepacker)
--
-- $TexturePacker:SmartUpdate:2d2724f3ec41f85115fca96e935747c5:1/1$
--
-- local sheetInfo = require("mysheet")
-- local myImageSheet = graphics.newImageSheet( "mysheet.png", sheetInfo:getSheet() )
-- local sprite = display.newSprite( myImageSheet , {frames={sheetInfo:getFrameIndex("sprite")}} )
--

local SheetInfo = {}

SheetInfo.sheet =
{
    frames = {
    
        {
            -- brick_greyscale_defense
            x=548,
            y=74,
            width=32,
            height=32,

        },
        {
            -- brick_greyscale_gem
            x=374,
            y=308,
            width=80,
            height=44,

        },
        {
            -- brick_greyscale_light
            x=290,
            y=308,
            width=80,
            height=44,

        },
        {
            -- brick_greyscale_normal
            x=464,
            y=74,
            width=80,
            height=44,

        },
        {
            -- btn_cup
            x=944,
            y=354,
            width=76,
            height=80,

            sourceX = 4,
            sourceY = 2,
            sourceWidth = 84,
            sourceHeight = 84
        },
        {
            -- btn_normal_blue
            x=4,
            y=476,
            width=278,
            height=116,

            sourceX = 4,
            sourceY = 2,
            sourceWidth = 284,
            sourceHeight = 120
        },
        {
            -- btn_normal_green
            x=288,
            y=472,
            width=280,
            height=116,

            sourceX = 2,
            sourceY = 2,
            sourceWidth = 284,
            sourceHeight = 120
        },
        {
            -- btn_normal_pink
            x=4,
            y=356,
            width=280,
            height=116,

            sourceX = 2,
            sourceY = 2,
            sourceWidth = 284,
            sourceHeight = 120
        },
        {
            -- btn_small_black
            x=744,
            y=354,
            width=196,
            height=118,

            sourceX = 2,
            sourceY = 2,
            sourceWidth = 200,
            sourceHeight = 120
        },
        {
            -- btn_small_blue
            x=572,
            y=476,
            width=184,
            height=118,

            sourceX = 8,
            sourceY = 2,
            sourceWidth = 200,
            sourceHeight = 120
        },
        {
            -- btn_small_grey
            x=828,
            y=156,
            width=188,
            height=118,

            sourceX = 6,
            sourceY = 2,
            sourceWidth = 200,
            sourceHeight = 120
        },
        {
            -- btn_tall_blue
            x=458,
            y=310,
            width=282,
            height=158,

            sourceX = 4,
            sourceY = 4,
            sourceWidth = 288,
            sourceHeight = 164
        },
        {
            -- btn_tall_grey
            x=4,
            y=194,
            width=282,
            height=158,

            sourceX = 4,
            sourceY = 4,
            sourceWidth = 288,
            sourceHeight = 164
        },
        {
            -- btn_wide2_grey
            x=4,
            y=74,
            width=456,
            height=116,

            sourceX = 2,
            sourceY = 2,
            sourceWidth = 460,
            sourceHeight = 120
        },
        {
            -- btn_wide_green
            x=464,
            y=190,
            width=316,
            height=116,

            sourceX = 2,
            sourceY = 2,
            sourceWidth = 320,
            sourceHeight = 120
        },
        {
            -- btn_wide_grey
            x=600,
            y=4,
            width=368,
            height=114,

            sourceX = 6,
            sourceY = 4,
            sourceWidth = 380,
            sourceHeight = 120
        },
        {
            -- gem_large
            x=352,
            y=394,
            width=60,
            height=60,

            sourceX = 2,
            sourceY = 2,
            sourceWidth = 64,
            sourceHeight = 64
        },
        {
            -- gem_small
            x=972,
            y=120,
            width=32,
            height=32,

        },
        {
            -- hint
            x=464,
            y=122,
            width=360,
            height=64,

        },
        {
            -- logo
            x=4,
            y=4,
            width=592,
            height=66,

            sourceX = 6,
            sourceY = 8,
            sourceWidth = 600,
            sourceHeight = 80
        },
        {
            -- paddle_ball
            x=972,
            y=84,
            width=36,
            height=32,

            sourceX = 2,
            sourceY = 4,
            sourceWidth = 40,
            sourceHeight = 40
        },
        {
            -- paddle_small_laser
            x=290,
            y=270,
            width=144,
            height=34,

        },
        {
            -- paddle_small_normal
            x=290,
            y=232,
            width=144,
            height=34,

        },
        {
            -- paddle_small_normal_wide
            x=744,
            y=316,
            width=220,
            height=34,

        },
        {
            -- paddle_small_shadow
            x=290,
            y=194,
            width=144,
            height=34,

        },
        {
            -- paddle_small_shadow_wide
            x=784,
            y=278,
            width=220,
            height=34,

        },
        {
            -- powerup_laser
            x=1012,
            y=4,
            width=8,
            height=32,

            sourceX = 0,
            sourceY = 4,
            sourceWidth = 8,
            sourceHeight = 40
        },
        {
            -- powerup_pill_bomb
            x=288,
            y=356,
            width=72,
            height=34,

            sourceX = 4,
            sourceY = 2,
            sourceWidth = 80,
            sourceHeight = 40
        },
        {
            -- powerup_pill_extend
            x=364,
            y=356,
            width=70,
            height=34,

            sourceX = 6,
            sourceY = 2,
            sourceWidth = 80,
            sourceHeight = 40
        },
        {
            -- powerup_pill_laser
            x=944,
            y=438,
            width=72,
            height=34,

            sourceX = 4,
            sourceY = 2,
            sourceWidth = 80,
            sourceHeight = 40
        },
        {
            -- settings
            x=288,
            y=394,
            width=60,
            height=64,

            sourceX = 2,
            sourceY = 0,
            sourceWidth = 64,
            sourceHeight = 64
        },
        {
            -- upgrade_block_lit
            x=972,
            y=44,
            width=36,
            height=36,

            sourceX = 2,
            sourceY = 2,
            sourceWidth = 40,
            sourceHeight = 40
        },
        {
            -- upgrade_block_unlit
            x=972,
            y=4,
            width=36,
            height=36,

            sourceX = 2,
            sourceY = 2,
            sourceWidth = 40,
            sourceHeight = 40
        },
    },
    
    sheetContentWidth = 1024,
    sheetContentHeight = 1024
}

SheetInfo.frameIndex =
{

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
}

function SheetInfo:getSheet()
    return self.sheet;
end

function SheetInfo:getFrameIndex(name)
    return self.frameIndex[name];
end

return SheetInfo
