import PIL
import PIL.Image

# DIR = r"C:\Users\fuzzl\Downloads\minecraft\texture packs\1.21\assets\minecraft\textures\particle"
DIR = r"C:\Users\fuzzl\Downloads\minecraft\texture packs\Faithful 32x - 1.21.4 Experimental\assets\minecraft\textures\particle"
TEX = "spell_{}.png"

MAX = 8

first = PIL.Image.open(f"{DIR}/{TEX.format(0)}")
atlas_size = (first.size[0] * MAX, first.size[1])

atlas = PIL.Image.new("RGBA", atlas_size)

for i in range(0, MAX):
    tex = TEX.format(i)
    img = PIL.Image.open(f"{DIR}/{tex}")
    atlas.paste(img, (i * first.size[1], 0))
    # print(img.size)
# atlas.show()
atlas.save(TEX.format("atlas_64"))
