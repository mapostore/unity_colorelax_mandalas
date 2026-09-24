import AppKit
import Foundation

guard CommandLine.arguments.count >= 2,
      let image = NSImage(contentsOfFile: CommandLine.arguments[1]),
      let cgImage = image.cgImage(forProposedRect: nil, context: nil, hints: nil) else {
    fputs("Usage: detect_mandala_regions <screenshot.png>\n", stderr)
    exit(2)
}

let bitmap = NSBitmapImageRep(cgImage: cgImage)

let width = bitmap.pixelsWide
let height = bitmap.pixelsHigh
let minX = 10, maxX = min(709, width - 1)
let minY = 120, maxY = min(817, height - 1)
let centerX = 360.0, centerY = 468.0
var white = Array(repeating: false, count: width * height)
var visited = Array(repeating: false, count: width * height)

for y in minY...maxY {
    for x in minX...maxX {
        guard let color = bitmap.colorAt(x: x, y: y)?.usingColorSpace(.deviceRGB) else { continue }
        white[y * width + x] = color.redComponent > 0.94 && color.greenComponent > 0.94 && color.blueComponent > 0.94
    }
}

if CommandLine.arguments.contains("--grid") {
    for y in stride(from: minY + 3, through: maxY - 3, by: 7) {
        for x in stride(from: minX + 3, through: maxX - 3, by: 7) {
            let dx = Double(x) - centerX, dy = Double(y) - centerY
            let radius = hypot(dx, dy)
            guard radius < 344, white[y * width + x] else { continue }
            let color = Int(radius / 42.0) % 5
            print("\(color),\(x),\(y),1")
        }
    }
    exit(0)
}

let neighbors = [(1, 0), (-1, 0), (0, 1), (0, -1)]
for startY in minY...maxY {
    for startX in minX...maxX {
        let start = startY * width + startX
        if visited[start] || !white[start] { continue }
        visited[start] = true
        var queue = [(startX, startY)]
        var cursor = 0
        var pixels: [(Int, Int)] = []
        var touchesEdge = false
        while cursor < queue.count {
            let (x, y) = queue[cursor]
            cursor += 1
            pixels.append((x, y))
            if x == minX || x == maxX || y == minY || y == maxY { touchesEdge = true }
            for (dx, dy) in neighbors {
                let nx = x + dx, ny = y + dy
                if nx < minX || nx > maxX || ny < minY || ny > maxY { continue }
                let index = ny * width + nx
                if !visited[index] && white[index] {
                    visited[index] = true
                    queue.append((nx, ny))
                }
            }
        }
        guard !touchesEdge, pixels.count >= 18 else { continue }
        let sx = pixels.reduce(0) { $0 + $1.0 }
        let sy = pixels.reduce(0) { $0 + $1.1 }
        let x = Double(sx) / Double(pixels.count)
        let y = Double(sy) / Double(pixels.count)
        let radius = hypot(x - centerX, y - centerY)
        guard radius < 345 else { continue }
        // Symmetric counterparts share radius and approximate area, so they receive the same color.
        let radialBand = Int(radius / 48.0)
        let areaBand = min(4, Int(log2(Double(pixels.count)) / 2.0))
        let color = (radialBand + areaBand) % 5
        print("\(color),\(Int(x.rounded())),\(Int(y.rounded())),\(pixels.count)")
    }
}
