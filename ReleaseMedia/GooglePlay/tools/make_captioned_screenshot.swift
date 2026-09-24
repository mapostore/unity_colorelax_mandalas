import AppKit
import Foundation

guard CommandLine.arguments.count == 5 else {
    fputs("Usage: make_captioned_screenshot <input.png> <output.jpg> <title> <subtitle>\n", stderr)
    exit(2)
}

let inputURL = URL(fileURLWithPath: CommandLine.arguments[1])
let outputURL = URL(fileURLWithPath: CommandLine.arguments[2])
let title = CommandLine.arguments[3]
let subtitle = CommandLine.arguments[4]

guard let screenshot = NSImage(contentsOf: inputURL) else {
    fputs("Unable to read screenshot\n", stderr)
    exit(1)
}

let canvasSize = NSSize(width: 1080, height: 1920)
let canvas = NSImage(size: canvasSize)
canvas.lockFocus()

NSColor(calibratedRed: 23 / 255, green: 24 / 255, blue: 29 / 255, alpha: 1).setFill()
NSBezierPath(rect: NSRect(origin: .zero, size: canvasSize)).fill()

// Quiet concentric accents connect screenshots to the mandala identity without competing with the UI.
for index in 0..<5 {
    let inset = CGFloat(index) * 42
    let accentRect = NSRect(x: -180 + inset, y: 1260 + inset, width: 620 - inset * 2, height: 620 - inset * 2)
    NSColor(calibratedRed: 0.05, green: 0.78, blue: 0.74, alpha: 0.035).setStroke()
    let accent = NSBezierPath(ovalIn: accentRect)
    accent.lineWidth = 4
    accent.stroke()
}

let paragraph = NSMutableParagraphStyle()
paragraph.alignment = .center

let titleAttributes: [NSAttributedString.Key: Any] = [
    .font: NSFont.systemFont(ofSize: 66, weight: .semibold),
    .foregroundColor: NSColor(calibratedRed: 1.0, green: 0.95, blue: 0.83, alpha: 1),
    .paragraphStyle: paragraph
]
let subtitleAttributes: [NSAttributedString.Key: Any] = [
    .font: NSFont.systemFont(ofSize: 31, weight: .regular),
    .foregroundColor: NSColor(calibratedRed: 0.69, green: 0.76, blue: 0.78, alpha: 1),
    .paragraphStyle: paragraph
]

(title as NSString).draw(in: NSRect(x: 70, y: 1760, width: 940, height: 90), withAttributes: titleAttributes)
(subtitle as NSString).draw(in: NSRect(x: 90, y: 1704, width: 900, height: 48), withAttributes: subtitleAttributes)

let screenshotHeight: CGFloat = 1600
let screenshotWidth = screenshotHeight * screenshot.size.width / screenshot.size.height
let screenshotRect = NSRect(
    x: (canvasSize.width - screenshotWidth) / 2,
    y: 55,
    width: screenshotWidth,
    height: screenshotHeight
)

NSGraphicsContext.saveGraphicsState()
let shadow = NSShadow()
shadow.shadowColor = NSColor.black.withAlphaComponent(0.48)
shadow.shadowBlurRadius = 28
shadow.shadowOffset = NSSize(width: 0, height: -10)
shadow.set()
NSColor.black.setFill()
NSBezierPath(roundedRect: screenshotRect, xRadius: 34, yRadius: 34).fill()
NSGraphicsContext.restoreGraphicsState()

NSGraphicsContext.saveGraphicsState()
NSBezierPath(roundedRect: screenshotRect, xRadius: 34, yRadius: 34).addClip()
screenshot.draw(in: screenshotRect, from: .zero, operation: .sourceOver, fraction: 1)
NSGraphicsContext.restoreGraphicsState()

canvas.unlockFocus()

guard let tiff = canvas.tiffRepresentation,
      let bitmap = NSBitmapImageRep(data: tiff),
      let jpeg = bitmap.representation(using: .jpeg, properties: [.compressionFactor: 0.94]) else {
    fputs("Unable to encode screenshot\n", stderr)
    exit(1)
}

try jpeg.write(to: outputURL)
