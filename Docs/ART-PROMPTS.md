# Registro de geração de arte (PT-BR)

## Proveniência

Ferramenta: geração de imagens integrada. A arte atual foi gerada para RunePact e não incorpora assets de Axie, Warcraft ou outras franquias de terceiros. Este registro não é aconselhamento jurídico; revise requisitos de uso comercial antes da publicação.

## Arquivos aprovados

- `Assets/Resources/Art/Warriors.png`: atlas de produção 3×2 de guerreiros.
- `Assets/Resources/Art/Arena.png`: arena de batalha de produção.
- `ArtDirection/Approved-v1/`: cópias imutáveis de referência aprovada.

## Prompt do atlas de guerreiros

```text
Use case: stylized-concept. Asset type: production sprite atlas for an original 2D pixel-art fantasy tactical card game, not a mockup. Generate a single transparent PNG atlas, landscape 1536x1024, precisely 3 columns x 2 rows of equal cells, each cell 512x512. Six unique full-body warrior sprites, one centered in each cell, with transparent generous margins, every figure entirely within its cell, feet around 85% cell height and head around 15%. Crisp deliberate chunky pixel art, dark pixel outlines, limited tasteful palette, no antialiasing, no words, no letters, no grid lines, no ground, no background, genuine alpha transparency (not checkerboard). All characters three-quarter view facing RIGHT, compact readable proportions 3 heads tall, approximately 96-pixel sprite detail scaled up. Top row left: teal-cloaked woman knight with bronze plate, straight sword and large kite shield. Top middle: indigo scholar mage, broad crescent-shaped brim hat, pale hair, wooden staff topped with amber crystal. Top right: rust-red hooded ranger with crossbow and leather satchel. Bottom left: imposing iron-armored raider with crimson cape, axe and angular shield. Bottom middle: dark violet masked sorcerer with angular crown and glowing purple lantern staff. Bottom right: moss-green masked rogue with twin curved daggers, leaflike cloak. Each visibly distinct. Original character designs only, not Axie, Warcraft, or any existing character. Intended to be sliced into exactly six game sprites in Unity. Do not include any other object outside each character silhouette.
```

A primeira tentativa de transparência não era utilizável. O atlas de produção selecionado usa um fundo magenta sólido de chroma-key removido em tempo de execução; veja o guia de estilo para as restrições.

## Prompt da arena

```text
Use case: stylized-concept. Asset type: original 2D pixel art background for a landscape 3-versus-3 warrior card battle. 1536x1024 canvas. No characters, no UI, no words. Side-view RPG battle arena in an ancient verdant mountain sanctuary at blue twilight. Upper half distant layered pine forest, broken archways, warm amber floating lanterns, dusky turquoise sky, tiny fireflies. Lower half broad flat weathered stone arena floor with restrained engraved circles, viewed in slight elevated perspective. CLEAR OPEN FLOOR left and right for six large character sprites. Center distant ruined sundial with amber crystal, subtle not dominating. Pixel-art with deliberate square clusters, detailed premium indie game environmental art, muted navy shadows, desaturated teal stone, warm gold highlights and tiny sage plants. Readable uncluttered composition, atmospheric depth, no blur or painterly brushwork. Original environment, no recognizable franchise assets.
```

Arte futura deve usar as imagens aprovadas em `ArtDirection/Approved-v1/` como referências visuais; este prompt sozinho não é suficiente para preservar o resultado aprovado.

---

# Art generation record (EN)

## Provenance

Tool: integrated image generation. The current artwork was generated for RunePact and does not incorporate Axie, Warcraft, or other third-party franchise assets. This record is not legal advice; review commercial-use requirements before publication.

## Approved files

- `Assets/Resources/Art/Warriors.png`: production 3×2 warrior atlas.
- `Assets/Resources/Art/Arena.png`: production battle arena.
- `ArtDirection/Approved-v1/`: immutable approval reference copies.

## Warrior atlas prompt

```text
Use case: stylized-concept. Asset type: production sprite atlas for an original 2D pixel-art fantasy tactical card game, not a mockup. Generate a single transparent PNG atlas, landscape 1536x1024, precisely 3 columns x 2 rows of equal cells, each cell 512x512. Six unique full-body warrior sprites, one centered in each cell, with transparent generous margins, every figure entirely within its cell, feet around 85% cell height and head around 15%. Crisp deliberate chunky pixel art, dark pixel outlines, limited tasteful palette, no antialiasing, no words, no letters, no grid lines, no ground, no background, genuine alpha transparency (not checkerboard). All characters three-quarter view facing RIGHT, compact readable proportions 3 heads tall, approximately 96-pixel sprite detail scaled up. Top row left: teal-cloaked woman knight with bronze plate, straight sword and large kite shield. Top middle: indigo scholar mage, broad crescent-shaped brim hat, pale hair, wooden staff topped with amber crystal. Top right: rust-red hooded ranger with crossbow and leather satchel. Bottom left: imposing iron-armored raider with crimson cape, axe and angular shield. Bottom middle: dark violet masked sorcerer with angular crown and glowing purple lantern staff. Bottom right: moss-green masked rogue with twin curved daggers, leaflike cloak. Each visibly distinct. Original character designs only, not Axie, Warcraft, or any existing character. Intended to be sliced into exactly six game sprites in Unity. Do not include any other object outside each character silhouette.
```

The first transparency pass was not usable. The selected production atlas uses a solid magenta chroma-key background which is removed at runtime; see the style guide for constraints.

## Arena prompt

```text
Use case: stylized-concept. Asset type: original 2D pixel art background for a landscape 3-versus-3 warrior card battle. 1536x1024 canvas. No characters, no UI, no words. Side-view RPG battle arena in an ancient verdant mountain sanctuary at blue twilight. Upper half distant layered pine forest, broken archways, warm amber floating lanterns, dusky turquoise sky, tiny fireflies. Lower half broad flat weathered stone arena floor with restrained engraved circles, viewed in slight elevated perspective. CLEAR OPEN FLOOR left and right for six large character sprites. Center distant ruined sundial with amber crystal, subtle not dominating. Pixel-art with deliberate square clusters, detailed premium indie game environmental art, muted navy shadows, desaturated teal stone, warm gold highlights and tiny sage plants. Readable uncluttered composition, atmospheric depth, no blur or painterly brushwork. Original environment, no recognizable franchise assets.
```

Future artwork must use the approved images in `ArtDirection/Approved-v1/` as visual references; this prompt alone is not enough to preserve the approved outcome.
