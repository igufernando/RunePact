# RunePact Repository Guide / Guia do repositório RunePact

## Scope / Escopo

This repository is the canonical local source of truth for RunePact. Do not develop from previous Codex workspace copies. The Tower Defense project is paused and is outside this repository.

Este repositório é a fonte local canônica do RunePact. Não desenvolva a partir das cópias anteriores da área de trabalho do Codex. O projeto Tower Defense está pausado e fica fora deste repositório.

## Language policy / Política de idioma

- Gameplay UI and player-facing text stay in Brazilian Portuguese for now.
- A interface do jogo e todo texto para jogadores permanecem em português brasileiro por enquanto.
- Commit subjects and bodies, code comments, technical documentation, and design documentation must be bilingual: English first, then Portuguese.
- Títulos e corpos dos commits, comentários de código, documentação técnica e documentação de design devem ser bilíngues: inglês primeiro, depois português.
- Keep identifiers, file names, and API symbols in English unless a third-party format requires otherwise.
- Mantenha identificadores, nomes de arquivos e símbolos de API em inglês, salvo quando um formato de terceiros exigir o contrário.

## Art direction / Direção de arte

The approved character style is locked until the user explicitly changes it. Read `Docs/ArtDirection/STYLE-GUIDE.md` and inspect `Docs/ArtDirection/Approved-v1/` before creating or replacing artwork.

O estilo aprovado dos personagens está bloqueado até uma mudança explícita do usuário. Leia `Docs/ArtDirection/STYLE-GUIDE.md` e inspecione `Docs/ArtDirection/Approved-v1/` antes de criar ou substituir arte.

## Project layout / Estrutura do projeto

- `Assets/Scripts/Core/`: pure combat rules / regras puras de combate.
- `Assets/Scripts/Presentation/`: Unity UI, input, and presentation / interface Unity, entrada e apresentação.
- `Assets/Editor/`: editor-only build and test tooling / ferramentas de build e testes exclusivas do editor.
- `Assets/Resources/Art/`: runtime game art / arte usada em tempo de execução.
- `Docs/`: design, art, and QA documentation / documentação de design, arte e QA.

## Verification / Verificação

Run `RunePact > Test combat rules` in Unity after gameplay changes. Do not commit `Library`, `Builds`, logs, or generated test result files.

Execute `RunePact > Test combat rules` na Unity após mudanças de gameplay. Não faça commit de `Library`, `Builds`, logs ou arquivos gerados de resultado de testes.
