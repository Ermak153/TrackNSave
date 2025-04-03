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
          errorMessage.value = "Данные чека ещё не получены или данные неверны";
          break;
        case 400:
          errorMessage.value = handleBadRequest(serverMessage);
          break;
        case 401:
          errorMessage.value = "Пользователь не аутентифицирован";
          break;
        case 403:
          errorMessage.value = handleForbidden(serverMessage);
          break;
        case 404:
          errorMessage.value = "Чек не найден";
          break;
        case 409:
          errorMessage.value = "Этот чек уже был добавлен ранее";
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
          errorMessage.value = "На данный момент данные о чеке недоступны, повторите попытку позже";
          break;
        default:
          errorMessage.value = "Неизвестная ошибка при обработке чека";
      }
    } else {
      errorMessage.value = "Ошибка соединения с сервером.";
    }
  };

  const handleBadRequest = (message?: string) => {
    const messages: Record<string, string> = {
      "Invalid QR code data": "Некорректный QR-код",
      "Invalid receipt": "Некорректный чек",
      "Couldn't get receipt details": "Не удалось получить данные чека",
      "Couldn't recognize the receipt": "Чек не распознан",
      "Fiscal data not found": "Фискальные данные не найдены",
      "QR code data not available for this receipt": "Данные QR-кода не верны",
      "Invalid receipt ID": "Неверный ID чека"
    };
    return messages[message || ""] || "Некорректные данные QR-кода или данные чека не распознаны";
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
    return messages[message || ""] || "Внутренняя ошибка сервера";
  }

  return { errorMessage, handleApiError };
}
