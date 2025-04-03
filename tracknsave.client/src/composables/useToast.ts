import { createApp } from 'vue';
import ToastTNS from '@/components/ToastTNS.vue';

let toastInstance: InstanceType<typeof ToastTNS> | null = null;

export function useToast() {
  if (!toastInstance) {
    const container = document.createElement('div');
    document.body.appendChild(container);
    const app = createApp(ToastTNS);
    toastInstance = app.mount(container) as InstanceType<typeof ToastTNS>;
  }

  return {
    show: (message: string, type: 'success' | 'error' | 'info') => {
      toastInstance?.showToast(message, type);
    }
  };
}
