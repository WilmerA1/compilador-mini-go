// Generated from c:/Users/Wil/OneDrive - Estudiantes ITCR/Semestre I 2026/Compi/proyecto/MiniGoCompiler/Grammar/MiniGo.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link MiniGoParser}.
 */
public interface MiniGoListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link MiniGoParser#root}.
	 * @param ctx the parse tree
	 */
	void enterRoot(MiniGoParser.RootContext ctx);
	/**
	 * Exit a parse tree produced by {@link MiniGoParser#root}.
	 * @param ctx the parse tree
	 */
	void exitRoot(MiniGoParser.RootContext ctx);
}