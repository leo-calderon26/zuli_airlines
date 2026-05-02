<template>
    <div class="flex min-w-0 flex-col gap-1">
        <label class="text-xs font-semibold text-slate-700">{{ label }}</label>
        <div class="relative">
            <div :class="containerClasses">
                <input
                    :value="modelValue"
                    :type="type"
                    :readonly="readonly"
                    :placeholder="placeholder"
                    :class="inputClasses"
                    @focus="showMenu = true"
                    @input="onInput"
                />
                <button
                    type="button"
                    :class="buttonClasses"
                    :aria-label="buttonLabel"
                    @click="toggleMenu"
                >
                    <img class="h-3.5 w-3.5" :src="arrowDownUrl" alt="" />
                </button>
            </div>
            <div
                v-if="showMenu"
                class="absolute left-0 right-0 top-full z-10 mt-1 max-h-44 overflow-y-auto rounded-md border border-slate-200 bg-[#f7f7f7] p-1 shadow-lg"
            >
                <button
                    v-for="option in options"
                    :key="option"
                    type="button"
                    class="block w-full rounded px-3 py-2 text-left text-sm text-slate-900 transition hover:bg-slate-300"
                    @click="selectOption(option)"
                >
                    {{ option }}
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import arrowDownUrl from '../../../assets/ArrowDown.svg'

export default {
    name: 'LandingDropdownField',
    props: {
        label: {
            type: String,
            required: true
        },
        modelValue: {
            type: String,
            default: ''
        },
        options: {
            type: Array,
            default: () => []
        },
        placeholder: {
            type: String,
            default: ''
        },
        type: {
            type: String,
            default: 'text'
        },
        readonly: {
            type: Boolean,
            default: false
        },
        inputClasses: {
            type: String,
            default: 'min-w-0 flex-1 bg-transparent px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 focus:outline-none'
        },
        buttonClasses: {
            type: String,
            default: 'flex w-11 shrink-0 items-center justify-center border-l border-slate-200 bg-[#f7f7f7] transition hover:bg-slate-200'
        },
        buttonLabel: {
            type: String,
            default: 'Mostrar opciones'
        }
        ,
        containerClasses: {
            type: String,
            default: 'flex items-stretch overflow-hidden rounded-md border border-slate-200 bg-[#f7f7f7]'
        }
    },
    emits: ['update:modelValue'],
    data() {
        return {
            showMenu: false,
            arrowDownUrl
        }
    },
    methods: {
        toggleMenu() {
            this.showMenu = !this.showMenu
        },
        selectOption(option) {
            this.$emit('update:modelValue', option)
            this.showMenu = false
        },
        onInput(event) {
            this.$emit('update:modelValue', event.target.value)
        }
    }
}
</script>