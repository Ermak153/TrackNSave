import { reactive } from "vue";
import api from "@/api/axios";

interface ReceiptItem {
  name: string;
  price: number;
  quantity: number;
  sum: number;
}

interface Receipt {
  id: string;
  createdAt: string;
  receiptData: {
    user: string;
    totalSum: number;
    dateTime: string;
    retailPlace: string;
    items: ReceiptItem[];
  };
}

const state = reactive({
  receipts: [] as Receipt[],
  errorMessage: null as string | null,
});

export const useReceipts = () => {
  const fetchReceipts = async () => {
    try {
      const response = await api.get("/receipt/list");
      state.receipts = response.data.formattedReceipts;
    } catch {
      state.errorMessage = "Ошибка при загрузке чеков";
    }
  };

  return {
    state,
    fetchReceipts,
  };
};
