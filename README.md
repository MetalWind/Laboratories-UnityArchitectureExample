# Laboratories - Архитектурный шаблон для Unity

Данный проект демонстрирует один из многих возможных подходов к построению архитектуры игры на Unity, который можно использовать как в качестве полноценного фундамента для вашей игры, так и в качестве источника определенных идей.

Laboratories не является идеальной, это то что использую я, и то чем я хотел бы поделиться, так как к этому моменту количество материала о организации кода в Unity, лично мне кажется удручающим. 

**Важное примечание:**

**Laboratories зависима от UniTask и Extenject. Если вы собираетесь использовать её, об этом стоит помнить.**



## Введение

Несмотря на то что я не слышал ранее именно такой формулировки, архитектура в моем понимании это скорее способ организации мыслей о коде, а не самого кода. Это то, что позволяет точно знать, где в иерархии связей находится класс над которым ты работаешь, что позволяет быстрее и качественнее интегрировать его в в систему. Из этого следует, что архитектура должна полно отвечать на вопросы о ролях и возможностях классов, составляя при этом целостную картину.

Я нередко сталкивался с тем, что с трудом могу обнаружить эту картину в играх других людей, возможно, в силу неопытности. Это натолкнуло меня на необходимость самому расставить всё по местам, и впоследствии, прийти к описанному ниже варианту архитектуры.



## Цель Laboratories

Нередко можно услышать нелестные вещи о том что объединяют под понятиями мэнеджеров и контроллеров, и в некоторой форме они и правда плохи. Однако, если полностью отказаться от них, то окажется что наша игра представляет собой огромнейший набор возможностей и инструментов, которые "некому" использовать. Лаборатории, это более управляемые и определенные в своём назначении контроллеры. Лаборатории, это та область кода которая имеет доступ к большинству инструментов и данных, объединяя сильно разрозненные части, для влияния на саму игру.

Стоит также сказать, что  лаборатории не видят различий в работе с обычными GameObject-ми и UI представлением. 

Хорошим примером использования лаборатории может быть инстанциирование всех игровых персонажей в их мир. Это слишком узкоспециализированная задача, чтобы давать это сервису; слишком объемная в коде, но незначительная в общем объеме задач чтобы давать её игровому состоянию. "Мэнеджеры" же, заточены под решение таких задач, и соответственно, лаборатории лишь развивают это качество.

## Начало работы

Как и говорилось в начале, вам нужно установить в проект Extenject и UniTask. Если вы ранее их не использовали, то рекомендую вначале ознакомиться с их документацией. После этого нужно добавить в ваш проект содержимое этого репозитория.

Чтобы представлять себе общую структуру того, c чем предстоит работать, предлагаю взглянуть на схему:
![LabDiagram](https://github.com/MetalWind/Laboratories-UnityArchitectureExample/assets/150790210/20788d22-4bae-4f2e-b7dc-0cc9a2c1f2ba)
Стоит взять во внимание что эта схема одновременно меньше и больше заложенного в коде, так как её цель, это дать представление о связях и предназначениях того или иного элемента.

## Управление игрой и её состоянием

У Laboratories есть несколько классов, от которых зависит что именно происходит в игре. Всё начинается с точки входа. Для того чтобы в вашей игре она работала, нужно просто добавить к любому активному объекту на сцене компонент EntryPoint. После этого нам понадобится провести подготовку Extenject. Она хорошо описана в его документации, и когда вы её закончите, то помимо собственных инсталлеров, добавьте в SceneContext CoreInstaller, он необходим для корректной работы архитектуры.

Теперь от достаточно формальных вещей можно перейти к более практическим. Нам необходимо настроить машину состояний. Для этого, понадобится реализовать абстрактный класс GameStateMachineBase, и в нем описать метод StartGame. Он может включать в себя некоторую дополнительную логику, но в большинстве случаев вы хотите сделать нечто такое:

```
    public override void StartGame() { Enter<ExampleFirstState>(); }
```

Далее нам нужно реализовать эти состояния, и в описании того как это должно выглядеть код справится лучше слов. 

Внутри состояния мы получаем полное моральное право на то чтобы использовать LabCenter, но перед тем как мы сделаем это, я покажу интерфейсы лабораторий

```
    public interface ILab
    {
        public UniTask Work();
        public UniTask Reboot();
        public UniTask Break();
    }
```

```
public interface IWarmedLab : ILab
{
    public UniTask WarmUp();
}
```

В этих интерфейсах явно прослеживается жизненный цикл лабораторий. Есть несколько способов управления им, однако предполагаемый выглядит так:

1. Если в списке лабораторий есть разогреваемые и не бывшие уже активными, то они поочередно разогреваются. Разогрев можно проводить и заранее, перед активацией лабораторий.

2. Лаборатории, которые были активны, но которых нет в новом списке, дезактивируются при помощи Break.

3. Повторяющиеся лаборатории, в зависимости от того в какой метод в LabCenter они попали, могут перезапуститься или просто быть проигнорированы. Перезапуск в большинстве случаев должен мочь выполняться просто при помощи. Разогрев для повторяющихся лабораторий не происходит.

4. Далее, лаборатории которые добавлены и не активны активируются методом Work, и он в отличие от WarmUp не ожидается, так как предполагается что все лаборатории работают параллельно.

Итак, теперь стоит поговорить о несколько путающей вещи. Нам ничего не мешает управлять всей игрой просто меняя набор активных лабораторий внутри одного состояния, однако, нередко это приводит к тому что состояние лишь запускает большое количество лабораторий, и они сами слушают события, запускаются, останавливаются, что в конечном итоге серьезно запутывает код.

Правило которому я обычно следую говорит о том, что если мне требуется искусственно замораживать работу лабораторий при помощи событий, то скорее всего такие события лучше отслеживать внутри состояния, и при необходимости сменять его. Если мне например необходимо отобразить панель магазина, управляемую лабораторией, то это можно сделать внутри состояния, но если требуется переключить  большое количество лабораторий, то это повод для создания нового состояния.



## Работа с сохраняемыми данными

За сохранение и доступ к данным в этой архитектуре отвечают DataCenter, и необходимое количество пар DataResolver и DataAccessor. Первый является точкой получения этих данных, второй за инициализацию третьего и его сохранение, третий же представляет общую точку доступа к этим данным. На примере инвентаря, InventoryDataResolver может порыться в ассетах, найти конфиг и создать на его основе класс данных отвечающий за содержимое инвентаря, а InventoryDataAccessor будет в каком то смысле самим инвентарем, предоставляя доступ к его содержимому, и его изменению. 

Предполагается, что Accessor-ы должны быть получены только лабораториями, в наборе который им необходим и Accessor-ми, которые их создают.

Единственное что осталось усвоить, это сохранение этих данных. Resolver в большинстве случаев, как уже упоминалось, должен сохранить в себе созданный Accessor. Это нужно для возможности сохранения данных Resolver-ми. Чтобы начать процесс сохранения, нужно вызвать событие SaveProcessInvokeSignal. После этого, LabCenter сначала сам запустит событие PrepareToSaveSignal, и когда оно окончится, вызовет у каждого Resolver-а метод SaveCurrent.
