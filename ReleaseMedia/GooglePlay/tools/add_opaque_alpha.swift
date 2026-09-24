import CoreGraphics
import Foundation
import ImageIO
import UniformTypeIdentifiers

guard CommandLine.arguments.count == 2 else {
    fputs("Usage: add_opaque_alpha.swift <png>\n", stderr)
    exit(2)
}

let fileURL = URL(fileURLWithPath: CommandLine.arguments[1])
guard let source = CGImageSourceCreateWithURL(fileURL as CFURL, nil),
      let image = CGImageSourceCreateImageAtIndex(source, 0, nil) else {
    fputs("Unable to read PNG\n", stderr)
    exit(1)
}

let colorSpace = CGColorSpaceCreateDeviceRGB()
guard let context = CGContext(
    data: nil,
    width: image.width,
    height: image.height,
    bitsPerComponent: 8,
    bytesPerRow: image.width * 4,
    space: colorSpace,
    bitmapInfo: CGImageAlphaInfo.premultipliedLast.rawValue
) else {
    fputs("Unable to create RGBA image\n", stderr)
    exit(1)
}

context.draw(image, in: CGRect(x: 0, y: 0, width: image.width, height: image.height))
guard let outputImage = context.makeImage(),
      let destination = CGImageDestinationCreateWithURL(
        fileURL as CFURL,
        UTType.png.identifier as CFString,
        1,
        nil
      ) else {
    fputs("Unable to create PNG destination\n", stderr)
    exit(1)
}

CGImageDestinationAddImage(destination, outputImage, nil)
guard CGImageDestinationFinalize(destination) else {
    fputs("Unable to write PNG\n", stderr)
    exit(1)
}
