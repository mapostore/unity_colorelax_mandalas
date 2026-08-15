--
-- created with TexturePacker (http://www.codeandweb.com/texturepacker)
--
-- $TexturePacker:SmartUpdate:a32673f7c922a23ad043b3bc56e7d210:1/1$
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
            x=274,
            y=37,
            width=16,
            height=16,

        },
        {
            -- brick_greyscale_gem
            x=187,
            y=154,
            width=40,
            height=22,

        },
        {
            -- brick_greyscale_light
            x=145,
            y=154,
            width=40,
            height=22,

        },
        {
            -- brick_greyscale_normal
            x=232,
            y=37,
            width=40,
            height=22,

        },
        {
            -- btn_cup
            x=472,
            y=177,
            width=38,
            height=40,

            sourceX = 2,
            sourceY = 1,
            sourceWidth = 42,
            sourceHeight = 42
        },
        {
            -- btn_normal_blue
            x=2,
            y=238,
            width=139,
            height=58,

            sourceX = 2,
            sourceY = 1,
            sourceWidth = 142,
            sourceHeight = 60
        },
        {
            -- btn_normal_green
            x=144,
            y=236,
            width=140,
            height=58,

            sourceX = 1,
            sourceY = 1,
            sourceWidth = 142,
            sourceHeight = 60
        },
        {
            -- btn_normal_pink
            x=2,
            y=178,
            width=140,
            height=58,

            sourceX = 1,
            sourceY = 1,
            sourceWidth = 142,
            sourceHeight = 60
        },
        {
            -- btn_small_black
            x=372,
            y=177,
            width=98,
            height=59,

            sourceX = 1,
            sourceY = 1,
            sourceWidth = 100,
            sourceHeight = 60
        },
        {
            -- btn_small_blue
            x=286,
            y=238,
            width=92,
            height=59,

            sourceX = 4,
            sourceY = 1,
            sourceWidth = 100,
            sourceHeight = 60
        },
        {
            -- btn_small_grey
            x=414,
            y=78,
            width=94,
            height=59,

            sourceX = 3,
            sourceY = 1,
            sourceWidth = 100,
            sourceHeight = 60
        },
        {
            -- btn_tall_blue
            x=229,
            y=155,
            width=141,
            height=79,

            sourceX = 2,
            sourceY = 2,
            sourceWidth = 144,
            sourceHeight = 82
        },
        {
            -- btn_tall_grey
            x=2,
            y=97,
            width=141,
            height=79,

            sourceX = 2,
            sourceY = 2,
            sourceWidth = 144,
            sourceHeight = 82
        },
        {
            -- btn_wide2_grey
            x=2,
            y=37,
            width=228,
            height=58,

            sourceX = 1,
            sourceY = 1,
            sourceWidth = 230,
            sourceHeight = 60
        },
        {
            -- btn_wide_green
            x=232,
            y=95,
            width=158,
            height=58,

            sourceX = 1,
            sourceY = 1,
            sourceWidth = 160,
            sourceHeight = 60
        },
        {
            -- btn_wide_grey
            x=300,
            y=2,
            width=184,
            height=57,

            sourceX = 3,
            sourceY = 2,
            sourceWidth = 190,
            sourceHeight = 60
        },
        {
            -- gem_large
            x=176,
            y=197,
            width=30,
            height=30,

            sourceX = 1,
            sourceY = 1,
            sourceWidth = 32,
            sourceHeight = 32
        },
        {
            -- gem_small
            x=486,
            y=60,
            width=16,
            height=16,

        },
        {
            -- hint
            x=232,
            y=61,
            width=180,
            height=32,

        },
        {
            -- logo
            x=2,
            y=2,
            width=296,
            height=33,

            sourceX = 3,
            sourceY = 4,
            sourceWidth = 300,
            sourceHeight = 40
        },
        {
            -- paddle_ball
            x=486,
            y=42,
            width=18,
            height=16,

            sourceX = 1,
            sourceY = 2,
            sourceWidth = 20,
            sourceHeight = 20
        },
        {
            -- paddle_small_laser
            x=145,
            y=135,
            width=72,
            height=17,

        },
        {
            -- paddle_small_normal
            x=145,
            y=116,
            width=72,
            height=17,

        },
        {
            -- paddle_small_normal_wide
            x=372,
            y=158,
            width=110,
            height=17,

        },
        {
            -- paddle_small_shadow
            x=145,
            y=97,
            width=72,
            height=17,

        },
        {
            -- paddle_small_shadow_wide
            x=392,
            y=139,
            width=110,
            height=17,

        },
        {
            -- powerup_laser
            x=506,
            y=2,
            width=4,
            height=16,

            sourceX = 0,
            sourceY = 2,
            sourceWidth = 4,
            sourceHeight = 20
        },
        {
            -- powerup_pill_bomb
            x=144,
            y=178,
            width=36,
            height=17,

            sourceX = 2,
            sourceY = 1,
            sourceWidth = 40,
            sourceHeight = 20
        },
        {
            -- powerup_pill_extend
            x=182,
            y=178,
            width=35,
            height=17,

            sourceX = 3,
            sourceY = 1,
            sourceWidth = 40,
            sourceHeight = 20
        },
        {
            -- powerup_pill_laser
            x=472,
            y=219,
            width=36,
            height=17,

            sourceX = 2,
            sourceY = 1,
            sourceWidth = 40,
            sourceHeight = 20
        },
        {
            -- settings
            x=144,
            y=197,
            width=30,
            height=32,

            sourceX = 1,
            sourceY = 0,
            sourceWidth = 32,
            sourceHeight = 32
        },
        {
            -- upgrade_block_lit
            x=486,
            y=22,
            width=18,
            height=18,

            sourceX = 1,
            sourceY = 1,
            sourceWidth = 20,
            sourceHeight = 20
        },
        {
            -- upgrade_block_unlit
            x=486,
            y=2,
            width=18,
            height=18,

            sourceX = 1,
            sourceY = 1,
            sourceWidth = 20,
            sourceHeight = 20
        },
    },
    
    sheetContentWidth = 512,
    sheetContentHeight = 512
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
