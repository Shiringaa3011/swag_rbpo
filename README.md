# swag_rbpo — CampusPass

Проект по РБПО.

- [`PROJECT.md`](PROJECT.md) — паспорт проекта
- [`security-requirements.md`](security-requirements.md) — требования безопасности `SR-*`
- [`THREAD_MODEL.md`](THREAD_MODEL.md) — модель угроз `T-*`
- [`design-decisions.md`](design-decisions.md) — проектные решения безопасности `D-*`
- [`S06_EVIDENCE.md`](S06_EVIDENCE.md) — подтверждение серверной валидации мероприятия
- [`S09_EVIDENCE.md`](S09_EVIDENCE.md) — S09: проверка SAST и истории Git на секреты, первичные результаты и ограничения
- [`S09_FINDINGS.md`](S09_FINDINGS.md) — S09: реестр решений и контекстных проверок
- [`CONTRIBUTIONS.md`](CONTRIBUTIONS.md) — вклад участников
- [`AI_USAGE.md`](AI_USAGE.md) — использование генеративного ИИ

## S08: сборка, тесты и стенд

Требуются .NET SDK, совместимый с [`global.json`](global.json), и Docker с Compose.

### Локальная сборка и тесты

```powershell
dotnet restore tests/CampusPass.Api.Tests/CampusPass.Api.Tests.csproj --locked-mode
dotnet build tests/CampusPass.Api.Tests/CampusPass.Api.Tests.csproj --configuration Release --no-restore
dotnet test tests/CampusPass.Api.Tests/CampusPass.Api.Tests.csproj --configuration Release --no-build --no-restore --logger "trx;LogFileName=s08-tests.trx" --results-directory TestResults
```

### Compose-стенд

```powershell
Copy-Item .env.example .env
docker compose config --quiet
docker compose up --build --wait --wait-timeout 180
```

Для macOS/Linux первая команда: `cp .env.example .env`.

Проверка работоспособности:

```powershell
Invoke-RestMethod http://localhost:8080/health
Invoke-RestMethod http://localhost:8080/info
Invoke-RestMethod http://localhost:8080/api/events
docker compose ps
docker compose exec api dotnet CampusPass.Api.dll --healthcheck
```

Ожидается: оба сервиса имеют статус `healthy`; `/health` возвращает `status=ok` и `database=ok`; `/info` — `label=campuspass-compose`; `/api/events` содержит демо-мероприятие.

Параметр `APP_LABEL` в `.env` — публичная настройка. Измените его, повторите `docker compose up --build --wait --wait-timeout 180` и сверьте ответ `/info`. Значения PostgreSQL в `.env.example` предназначены только для изолированного локального стенда.

Остановка без удаления данных: `docker compose down`. Полный сброс учебной базы: `docker compose down --volumes`.

### GitHub Actions

- `S08 Build and test` запускается для push, pull request и вручную. Он сохраняет TRX, опубликованное приложение и `checked_commit` в артефакте `s08-evidence-*`.
- `S08 Container check` запускается вручную и проверяет Compose-стенд, HTTP-ответы, непривилегированного пользователя образа и отсутствие `.env` в контейнере.

Фактический статус GitHub Actions подтверждается только прогоном после публикации изменений; локальные тесты его не заменяют.
