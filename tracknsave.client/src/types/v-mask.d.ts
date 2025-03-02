declare module 'v-mask' {
  import { Plugin } from 'vue';

  type MaskBindingValue = {
    expression: string;
  };

  type MaskBindingParameter = {
    value: MaskBindingValue;
    arg: string | null;
    modifiers: Record<string, boolean>;
    instance: unknown;
  };

  export const VueMaskPlugin: {
    install: (app: unknown) => void;
    VueMaskDirective: {
      bind: (el: HTMLElement, binding: MaskBindingParameter) => void;
    }
  };

  const VueMask: Plugin;
  export default VueMask;
}
