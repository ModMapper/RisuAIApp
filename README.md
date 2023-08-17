# Risu AI App
RisuAI MAUI App

Risu AI C# App porting version.

Requirement .NET 7

**Original Risu AI : https://github.com/kwaroran/RisuAI**

**Risu 최신버전 업데이트 절차**
1. 원본 Risu AI를 받아 노드 버전을 컴파일한다.
2. 컴파일 된 *"dist"* 폴더에 *"DragDropTouch.js"* 를 복사한다 (*"wwwroot"* 폴더 참조)
3. *"index.html"* 에 아래 스크립트를 추가한다.
   ```HTML
   <script>globalThis.__NODE__ = true</script>
   <script src="/DragDropTouch.js"></script>
   ```
4. *"wwwroot"* 폴더의 파일을 *"dist"* 폴더의 파일로 모두 대체한다.
5. 프로젝트 컴파일 후 배포.
