# RunePact — Approved Visual Identity v1 / Identidade visual aprovada v1

## Decision / Decisão

The user approved the current character asset style. Preserve it for all future characters, enemies, equipment variants, skins, and animations until the user explicitly requests a style change. Layout, UI, individual item designs, rules, and balance may evolve without changing this identity.

O usuário aprovou o estilo atual dos assets de personagens. Preserve-o para todos os personagens, inimigos, variantes de equipamentos, skins e animações futuros até uma mudança ser solicitada explicitamente. Layout, interface, itens individuais, regras e balanceamento podem evoluir sem alterar essa identidade.

## Canonical references / Referências canônicas

`Approved-v1/Warriors.png` is the canonical approved character atlas. `Approved-v1/Arena.png` records the gameplay context in which the characters were approved. The images take precedence over older prompts.

`Approved-v1/Warriors.png` é o atlas canônico aprovado dos personagens. `Approved-v1/Arena.png` registra o contexto de gameplay no qual os personagens foram aprovados. As imagens têm precedência sobre prompts antigos.

## Visual contract / Contrato visual

- Detailed medieval-fantasy pixel art, dark silhouettes, and crisp readable forms / pixel art detalhada de fantasia medieval, silhuetas escuras e formas nítidas.
- Full-body three-quarter battle view with comparable proportions, apparent scale, and foot alignment / vista de batalha em três quartos com corpo inteiro, proporções, escala aparente e alinhamento dos pés comparáveis.
- Blocky highlights on metal, angular cloth folds, warm leather, deep shadows, and restrained bright accents / reflexos em blocos no metal, dobras angulares de tecido, couro quente, sombras profundas e destaques luminosos contidos.
- Distinct yet coherent color families; avoid chibi, smooth painting, vector, photorealistic, or generic 3D render styles / famílias de cor distintas, mas coerentes; evite estilos chibi, pintura lisa, vetor, fotorrealismo ou render 3D genérico.
- Original designs only. Do not use recognizable characters, symbols, assets, or lore from other franchises / apenas designs originais. Não use personagens, símbolos, assets ou lore reconhecíveis de outras franquias.

## Technical contract / Contrato técnico

- Current source atlas: 1536×1024, 3 columns × 2 rows, 512×512 cells / atlas atual: 1536×1024, 3 colunas × 2 linhas, células de 512×512.
- Unity uses point filtering and no mipmaps for this artwork / a Unity usa filtragem Point e nenhum mipmap para essa arte.
- The magenta background in the current atlas is a chroma-key implementation detail, not visual palette / o fundo magenta no atlas atual é um detalhe de chroma-key, não uma cor da paleta visual.
- Prefer validated true alpha on new assets. Do not reorganize the atlas without updating and testing the loader / prefira alpha verdadeiro validado em novos assets. Não reorganize o atlas sem atualizar e testar o carregador.

## Required workflow / Fluxo obrigatório

1. Inspect the approved references before generating or replacing art / Inspecione as referências aprovadas antes de gerar ou substituir arte.
2. Supply them as style references to the image generator; for a variant of an existing hero, also provide that hero as an identity reference / Forneça-as ao gerador como referências de estilo; para uma variante de herói existente, forneça também esse herói como referência de identidade.
3. Request only the needed change while retaining perspective, materials, outlines, proportions, pixel density, and apparent scale / Solicite apenas a mudança necessária preservando perspectiva, materiais, contornos, proporções, densidade de pixels e escala aparente.
4. Save candidates separately with a version, prompt, date, and source references / Salve candidatos separadamente com versão, prompt, data e referências de origem.
5. Compare candidates in the battle scene at actual display size against at least two approved characters / Compare candidatos na cena de batalha, em escala real, contra ao menos dois personagens aprovados.
6. Obtain user approval before promoting any new baseline / Obtenha aprovação do usuário antes de promover uma nova base.

## Prompt scaffold / Molde de prompt

> Create [requested asset] for RunePact. The attached approved RunePact images are style references. Keep their three-quarter full-body battle view, proportions, dark outlines, material rendering, pixel-art detail density, contrast, and apparent scale. Change only [requested difference]. Original design, transparent background, no text, no logos, no external shadow, no chibi, vector, smooth painting, or generic 3D rendering.
>
> Crie [asset solicitado] para RunePact. As imagens aprovadas anexadas de RunePact são referências de estilo. Mantenha a vista de batalha em três quartos com corpo inteiro, proporções, contornos escuros, materiais, densidade de detalhes em pixel art, contraste e escala aparente. Altere apenas [diferença solicitada]. Design original, fundo transparente, sem texto, logos, sombra externa, chibi, vetor, pintura lisa ou render 3D genérico.

See [art prompt history](../ART-PROMPTS.md) for the original generation record.

Veja o [histórico de prompts de arte](../ART-PROMPTS.md) para o registro da geração original.
