import { ref } from "vue";
import type { AxiosError } from "axios";

interface ApiErrorResponse {
  message?: string;
}

export function useErrorHandler() {
  const errorMessage = ref<string | null>(null);

  const handleApiError = (error: AxiosError<ApiErrorResponse>) => {
    if (error.response) {
      const serverMessage = error.response.data?.message;
      switch (error.response.status) {
        case 202:
          errorMessage.value = handleAccepted(serverMessage);
          break;
        case 400:
          errorMessage.value = handleBadRequest(serverMessage);
          break;
        case 401:
          errorMessage.value = handleUnauthorize(serverMessage);
          break;
        case 403:
          errorMessage.value = handleForbidden(serverMessage);
          break;
        case 404:
          errorMessage.value = handleNotFound(serverMessage);
          break;
        case 409:
          errorMessage.value = handleConflict(serverMessage);
          break;
        case 429:
          errorMessage.value = handleTooManyRequests(serverMessage);
          break;
        case 500:
          errorMessage.value = handleServerError(serverMessage);
          break;
        case 502:
          errorMessage.value = handleBadGateway(serverMessage);
          break;
        case 503:
          errorMessage.value = handleServiceUnavailable(serverMessage);
          break;
        default:
          errorMessage.value = "Неизвестная ошибка";
      }
    } else {
      errorMessage.value = "Ошибка соединения с сервером";
    }
  };

  const handleServiceUnavailable = (message?: string) => {
    const messages: Record<string, string> = {
      "No receipt data available at the moment, retry later.": "На данный момент данные о чеке недоступны, повторите попытку позже",
    };
    return messages[message || ""] || "Сервер не готов обработать запрос";
  }

  const handleAccepted = (message?: string) => {
    const messages: Record<string, string> = {
      "The receipt data is not available yet, or the receipt is incorrect": "Данные чека ещё не получены или данные неверные",
    };
    return messages[message || ""] || "Данные получены, но в текущий момент не могут быть обработаны";
  }

  const handleConflict = (message?: string) => {
    const messages: Record<string, string> = {
      "Receipt has already been added earlier": "Этот чек уже был добавлен ранее",
      "Username already exists": "Это имя пользователя уже занято",
      "Email already exists": "Этот E-mail уже занят",
    };
    return messages[message || ""] || "Неизвестный конфликт";
  }

  const handleUnauthorize = (message?: string) => {
    const messages: Record<string, string> = {
      "Invalid username or password": "Неверное имя пользователя или пароль"
    };
    return messages[message || ""] || "Пользователь не аутентифицирован";
  }

  const handleBadRequest = (message?: string) => {
    const messages: Record<string, string> = {
      "Invalid QR code data": "Некорректный QR-код",
      "Invalid receipt": "Некорректный чек",
      "Couldn't get receipt details": "Не удалось получить данные чека",
      "Couldn't recognize the receipt": "Чек не распознан",
      "Fiscal data not found": "Фискальные данные не найдены",
      "QR code data not available for this receipt": "Данные QR-кода не верны",
      "Invalid receipt ID": "Неверный ID чека",
      "Only verified receipts can be download": "Только проверенные чеки могут быть скачаны",
      "Input JSON data cannot be null": "Входные данные JSON не могут быть пусты",
      "Formatted receipt data cannot be null": "Отформатированные данные чека не могу быть пусты",
      "Fiscal data cannot be null": "Фискальные данные не могут быть пусты",
      "QR code data cannot be null": "Данные QR-кода не могут быть пусты",
      "Token refresh error": "Ошибка обновления токена",
      "Invalid access token": "Неверный токен доступа",
      "Token is invalid or malformed": "Токен недействителен или неправильно сформирован",
      "Product name is required": "Требуется указать название продукта",
      "File size exceeds 5MB limit": "Размер файла не может превышать 5 Мб",
      "Invalid file extension": "Неверное разрешение файла"
    };
    return messages[message || ""] || "Некорректный запрос";
  };

  const handleTooManyRequests = (message?: string) => {
    const messages: Record<string, string> = {
      "Exceeded number of requests for this receipt": "Превышено количество запросов на получение этого чека",
      "Too many requests, wait before retrying": "Слишком много запросов, подождите, прежде чем повторить попытку",
    };
    return messages[message || ""] || "Слишком много запросов, попробуйте позже";
  };

  const handleServerError = (message?: string) => {
    const messages: Record<string, string> = {
      "Error when receiving receipt data": "Ошибка при получении данных чека",
      "Error processing receipt data": "Ошибка при обработке данных чека",
      "Unknown response code from API": "Неизвестная ошибка при обработке запроса",
      "Error generating receipt PDF": "Ошибка при создании PDF",
      "Error parsing JSON": "Ошибка при получении данных",
      "Error generating QRCode": "Ошибка создания QR-кода",
      "Error rendering receipt template": "Ошибка обработки шаблона чека",
      "Error updating receipt": "Ошибка при изменении чека",
      "Error saving receipt": "Ошибка при сохрании чека",
      "Refresh token not found or revoked": "Токен обновления не найден или отозван",
      "Error when trying to logout": "Ошибка при попытке выйти",
      "Failed to extract fiscal data": "Ошибка при получении фискальных данных",
      "Failed to extract QR code data": "Ошибка при получении данных QR-кода",
      "Required JSON fields 'data' or 'json' are missing": "Необходимые JSON поля 'data' или 'json' отсутствуют",
      "Failed to format receipt data": "Не удалось отформатировать данные чека",
      "Classification service returned empty response": "Сервис классификации вернул пустой ответ",
      "Failed to deserialize classification response": "Не удалось десериализовать ответ классификации",
      "An unexpected error occurred while saving the receipt": "При сохранении чека произошла непредвиденная ошибка",
      "An unexpected error occurred while deleting the receipt": "При удалении чека произошла непредвиденная ошибка",
      "An unexpected error occurred while updating the receipt": "При изменении чека произошла непредвиденная ошибка",
      "An unexpected error occurred during authorization": "При авторизации произошла непредвиденная ошибка",
      "Error generating access token": "Ошибка генерации токена доступа",
      "Error generating refresh token": "Ошибка генерации токена обновления",
      "Error saving refresh token": "Ошибка сохранения токена обновления",
      "Failed to revoke refresh token": "Не удалось отозвать токен обновления",
      "Error updating the user's avatar": "Ошибка при обновлении аватара пользователя",
      "Error saving avatar": "Ошибка сохранения аватара пользователя",
      "Error deleting avatar": "Ошибка при удалении аватара пользователя",
      "Error updating user avatar": "Ошибка при изменении аватара пользователя"

    };
    return messages[message || ""] || "Внутренняя ошибка сервера";
  };

  const handleBadGateway = (message?: string) => {
    const messages: Record<string, string> = {
      "Network error when contacting the receipt API": "Сетевая ошибка при обращении к API",
      "Receipt API returned error": "Неизвестная ошибка при обращении к API",
      "Error when receiving receipt data from API": "Ошибка при получении данных чека с API",
    };
    return messages[message || ""] || "Ошибка шлюза при запросе к API";
  };

  const handleForbidden = (message?: string) => {
    const messages: Record<string, string> = {
      "Only manual receipts can be edited": "Только вручную добавленные чеки могут быть изменены",
      "User access denied": "Доступ пользователю запрещён",
    };
    return messages[message || ""] || "Доступ пользователю запрещён";
  }

  const handleNotFound = (message?: string) => {
    const messages: Record<string, string> = {
      "Receipt not found": "Чек не найден",
      "Token not found": "Токен не найден",
      "User was not found": "Пользователь не найден"
    };
    return messages[message || "" || "По вашему запросу ничего не найдено"]
  }

  return { errorMessage, handleApiError };
}
