import { reactive, ref, computed } from "vue";
import api from "@/api/axios";

interface ReceiptItem {
  name: string;
  price: number;
  quantity: number;
  sum: number;
  category: string;
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

const sortOption = ref("Недавно добавленные");
const dateRange = ref<[string | null, string | null]>([null, null]);


export const useReceipts = () => {
  const totalReceipts = computed(() => state.receipts.length);

  const totalAmount = computed(() =>
    +(state.receipts.reduce((sum, r) => sum + (r.receiptData.totalSum || 0), 0) / 100).toFixed(2)
  );

  const mostPopularCategory = computed(() => {
  if (state.receipts.length === 0) return "Отсутствует";

  const categoryCounts: Record<string, number> = {};

  state.receipts.forEach(receipt => {
    receipt.receiptData.items.forEach(item => {
      if (item.category) {
        categoryCounts[item.category] = (categoryCounts[item.category] || 0) + 1;
      }
    });
  });

  const categories = Object.keys(categoryCounts);
  if (categories.length === 0) return "Отсутствует";

  return categories.reduce((a, b) =>
    categoryCounts[a] > categoryCounts[b] ? a : b
  );
});

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
    } catch {
      state.errorMessage = "Ошибка при удалении чека";
      return false;
    }
  };

  const filteredReceipts = computed(() => {
    const [start, end] = dateRange.value;

    let filtered = [...state.receipts];

    if (start && end) {
      const startDate = new Date(start).getTime();
      const endDate = new Date(end).getTime();

      filtered = filtered.filter(r => {
        const receiptDate = new Date(r.receiptData.dateTime).getTime();
        return receiptDate >= startDate && receiptDate <= endDate;
      });
    }

    switch (sortOption.value) {
      case "Сначала новые":
        return filtered.sort((a, b) =>
          new Date(b.receiptData.dateTime).getTime() - new Date(a.receiptData.dateTime).getTime()
        );
      case "Сначала старые":
        return filtered.sort((a, b) =>
          new Date(a.receiptData.dateTime).getTime() - new Date(b.receiptData.dateTime).getTime()
        );
      case "По сумме (возр.)":
        return filtered.sort((a, b) =>
          (a.receiptData.totalSum || 0) - (b.receiptData.totalSum || 0)
        );
      case "По сумме (убыв.)":
        return filtered.sort((a, b) =>
          (b.receiptData.totalSum || 0) - (a.receiptData.totalSum || 0)
        );
      case "Недавно добавленные":
      default:
        return filtered.sort((a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
    }
  });

  const setDateRange = (range: [string | null, string | null]) => {
    dateRange.value = range;
  };

  const setSortOption = (option: string) => {
    sortOption.value = option;
  };

  return {
    state,
    filteredReceipts,
    sortOption,
    totalReceipts,
    totalAmount,
    mostPopularCategory,
    setDateRange,
    fetchReceipts,
    deleteReceipt,
    setSortOption,
  };
};
