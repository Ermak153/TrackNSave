import { reactive } from "vue";
import api from "@/api/axios";

interface ReceiptItem {
  name: string;
  price: number;
  quantity: number;
  sum: number;
}

interface Receipt {
  id: number;
  createdAt: string;
  isVerified: boolean;
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

  const deleteReceipt = async (receiptId: number) => {
    try {
      await api.delete("/receipt/delete", {
        data: { ReceiptId: receiptId }
      });

      state.receipts = state.receipts.filter(receipt => receipt.id !== receiptId);

      state.errorMessage = null;
      return true;
    } catch (error) {
      state.errorMessage = "Ошибка при удалении чека";
      console.error("Delete error:", error);
      return false;
    }
  };

  return {
    state,
    fetchReceipts,
    deleteReceipt,
  };
};
